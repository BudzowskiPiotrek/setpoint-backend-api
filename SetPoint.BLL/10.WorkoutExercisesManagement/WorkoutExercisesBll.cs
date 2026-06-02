using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SetPoint.BLL._10.WorkoutExercisesManagement.Dto;
using SetPoint.DAL._1.Entity;
using SetPoint.DAL._2.Context;

namespace SetPoint.BLL._10.WorkoutExercisesManagement
{
    public class WorkoutExercisesBll : IWorkoutExercisesBll
    {
        #region Fields

        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly string _connectionString;

        #endregion


        #region Constructors

        public WorkoutExercisesBll(IConfiguration config)
        {
            _config = config;
            // Retrieve the connection string from configuration
            _connectionString = _config.GetConnectionString("PostgreConnection") ??
                 throw new InvalidOperationException("Connection string 'PostgreConnection' not found.");

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
            using (var context = new SetPointDbContext(_connectionString))
            {
                var existing = await context.WorkoutExercises
                    .FirstOrDefaultAsync(we => we.Id == dto.Id ||
                                              (we.SessionId == dto.SessionId &&
                                               we.ExerciseId == dto.ExerciseId &&
                                               we.Order == dto.Order));

                if (existing == null)
                {
                    var entity = _mapper.Map<WorkoutExercises>(dto);
                    await context.WorkoutExercises.AddAsync(entity);
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
                        context.WorkoutExercises.Update(existing);
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