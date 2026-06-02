using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SetPoint.BLL._09.WorkoutSessionsManagement.Dto;
using SetPoint.DAL._1.Entity;
using SetPoint.DAL._2.Context;

namespace SetPoint.BLL._09.WorkoutSessionsManagement
{
    public class WorkoutSessionsBll : IWorkoutSessionsBll
    {
        #region Fields

        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly string _connectionString;

        #endregion


        #region Constructors

        public WorkoutSessionsBll(IConfiguration config)
        {
            _config = config;
            // Retrieve the connection string from configuration
            _connectionString = _config.GetConnectionString("PostgreConnection") ??
                 throw new InvalidOperationException("Connection string 'PostgreConnection' not found.");

            // Configure AutoMapper
            var conMap = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<WorkoutSessionsDto, WorkoutSessions>();
                cfg.CreateMap<WorkoutSessions, WorkoutSessionsDto>();
            });
            _mapper = conMap.CreateMapper();
        }

        #endregion


        #region Methods
        public async Task<bool> SyncWorkoutSession(WorkoutSessionsDto dto)
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var existing = await context.WorkoutSessions
                    .FirstOrDefaultAsync(w => w.Id == dto.Id ||
                                             (w.UserId == dto.UserId && w.Date == dto.Date));

                if (existing == null)
                {
                    var entity = _mapper.Map<WorkoutSessions>(dto);
                    await context.WorkoutSessions.AddAsync(entity);
                }
                else
                {
                    if (existing.DeletedAt != null && dto.DeletedAt == null)
                    {
                        existing.DeletedAt = null;
                    }

                    if (dto.UpdatedAt > existing.UpdatedAt || existing.UpdatedAt == null)
                    {
                        _mapper.Map(dto, existing);
                        context.WorkoutSessions.Update(existing);
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