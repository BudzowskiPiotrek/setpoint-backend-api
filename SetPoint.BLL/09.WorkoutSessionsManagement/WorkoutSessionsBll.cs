using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SetPoint.BLL._09.WorkoutSessionsManagement.Dto;
using SetPoint.DAL._1.Entity;
using SetPoint.DAL._2.Context;

namespace SetPoint.BLL._09.WorkoutSessionsManagement
{
    public class WorkoutSessionsBll : IWorkoutSessionsBll
    {
        #region Fields
        private readonly IMapper _mapper;
        private readonly SetPointDbContext _context;
        #endregion


        #region Constructors
        public WorkoutSessionsBll(SetPointDbContext context)
        {
            _context = context;

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
            var existing = await _context.WorkoutSessions
                .FirstOrDefaultAsync(w => w.Id == dto.Id ||
                                         (w.UserId == dto.UserId && w.Date == dto.Date));

            if (existing == null)
            {
                var entity = _mapper.Map<WorkoutSessions>(dto);
                await _context.WorkoutSessions.AddAsync(entity);
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
                    _context.WorkoutSessions.Update(existing);
                }
                else
                {
                    return true;
                }
            }
            return await _context.SaveChangesAsync() > 0;
        }
        #endregion
    }
}