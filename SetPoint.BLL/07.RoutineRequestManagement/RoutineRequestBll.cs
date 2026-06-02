using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SetPoint.BLL._07.RoutineRequestManagement.Dto;
using SetPoint.BLL._07.RoutinesManagement;
using SetPoint.DAL._1.Entity;
using SetPoint.DAL._2.Context;

namespace SetPoint.BLL._07.RoutineRequestManagement
{
    public class RoutineRequestBll : IRoutineRequestBll
    {
        #region Fields

        private readonly IConfiguration _config;

        private readonly IMapper _mapper;

        private readonly string _connectionString;

        private readonly IRoutineBll _routine;

        #endregion


        #region Constructors

        public RoutineRequestBll(IConfiguration config, IRoutineBll routine)
        {
            _config = config;
            _routine = routine;
            // Retrieve the connection string from configuration
            _connectionString = _config.GetConnectionString("PostgreConnection") ??
                 throw new InvalidOperationException("Connection string 'PostgreConnection' not found.");

            // Configure AutoMapper
            var conMap = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RoutineRequestDto, RoutineRequests>();
            });
            _mapper = conMap.CreateMapper();
        }

        #endregion


        #region Methods

        public async Task<bool> SyncRoutineRequest(RoutineRequestDto dto)
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var existing = await context.RoutineRequests
                    .FirstOrDefaultAsync(r => r.Id == dto.Id ||
                                             (r.SenderId == dto.SenderId &&
                                              r.ReceiverId == dto.ReceiverId &&
                                              r.RoutineId == dto.RoutineId));

                if (existing == null)
                {
                    var entity = _mapper.Map<RoutineRequests>(dto);
                    await context.RoutineRequests.AddAsync(entity);
                }
                else
                {
                    if (existing.Status == RequestStatus.Rejected)
                    {
                        _mapper.Map(dto, existing);
                        existing.Status = RequestStatus.Pending;
                        existing.UpdatedAt = DateTime.UtcNow;
                        context.RoutineRequests.Update(existing);
                    }
                    else if (dto.UpdatedAt > existing.UpdatedAt || existing.UpdatedAt == null)
                    {
                        if (existing.DeletedAt != null && dto.DeletedAt == null)
                        {
                            existing.DeletedAt = null;
                        }

                        if (dto.Status == RequestStatus.Accepted)
                        {
                            var copy = await _routine.CloneRoutineForUserAsync(existing.RoutineId, existing.ReceiverId);
                            if (!copy) return false;
                        }
                        _mapper.Map(dto, existing);
                        context.RoutineRequests.Update(existing);
                    }
                    else
                    {
                        return true;
                    }
                }
                return await context.SaveChangesAsync() > 0;
            }
        }
        #endregion
    }
}
