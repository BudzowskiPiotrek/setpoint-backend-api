using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SetPoint.BLL._10.WorkoutExercisesManagement.Dto;
using SetPoint.DAL._1.Entity;
using SetPoint.DAL._2.Context;

namespace SetPoint.BLL._10.WorkoutExercisesManagement
{
    public class WorkoutExercisesBll : IWorkoutExercisesBll
    {
        #region Fields
        private readonly IMapper _mapper;
        private readonly SetPointDbContext _context;
        #endregion


        #region Constructors
        public WorkoutExercisesBll(SetPointDbContext context)
        {
            _context = context;

            // Configure AutoMapper
            var conMap = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<WorkoutExercisesDto, WorkoutExercises>();
                cfg.CreateMap<WorkoutExercises, WorkoutExercisesDto>();
            });
            _mapper = conMap.CreateMapper();
        }
        #endregion


        #region Methods
        public async Task<bool> SyncWorkoutExercise(WorkoutExercisesDto dto)
        {
            var existing = await _context.WorkoutExercises
                .FirstOrDefaultAsync(we => we.Id == dto.Id ||
                                          (we.SessionId == dto.SessionId &&
                                           we.ExerciseId == dto.ExerciseId &&
                                           we.Order == dto.Order));

            if (existing == null)
            {
                var entity = _mapper.Map<WorkoutExercises>(dto);
                await _context.WorkoutExercises.AddAsync(entity);
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
                    _context.WorkoutExercises.Update(existing);
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