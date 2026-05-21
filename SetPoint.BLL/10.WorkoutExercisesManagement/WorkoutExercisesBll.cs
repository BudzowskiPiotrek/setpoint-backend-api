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

        public async Task<bool> CreateWorkoutExercise(WorkoutExercisesDto workoutDto)
        {
            if (workoutDto == null)
                throw new Exception("WorkoutExercisesDto cannot be null.");

            using (var context = new SetPointDbContext(_connectionString))
            {
                var exists = await context.WorkoutExercises.AnyAsync(r =>
                    r.ExerciseId == workoutDto.ExerciseId && r.SessionId == workoutDto.SessionId && r.Order == workoutDto.Order && r.DeletedAt == null);

                if (exists)
                    throw new InvalidOperationException("A workout exercise with the same exercise, session, and order already exists.");

                var entity = _mapper.Map<WorkoutExercises>(workoutDto);

                if (entity.Id == Guid.Empty)
                    entity.Id = Guid.NewGuid();

                entity.CreatedAt = DateTime.UtcNow;

                await context.WorkoutExercises.AddAsync(entity);

                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> UpdateWorkoutExercise(WorkoutExercisesDto workoutDto)
        {
            if (workoutDto == null)
                throw new Exception("WorkoutExercisesDto cannot be null.");

            using (var context = new SetPointDbContext(_connectionString))
            {
                var exists = await context.WorkoutExercises.AnyAsync(r =>
                    r.ExerciseId == workoutDto.ExerciseId && r.SessionId == workoutDto.SessionId && r.Order == workoutDto.Order && r.DeletedAt == null);

                if (exists)
                    throw new InvalidOperationException("A workout exercise with the same exercise, session, and order already exists.");

                var entity = await context.WorkoutExercises.FirstOrDefaultAsync(we => we.Id == workoutDto.Id && we.DeletedAt == null);

                if (entity == null)
                    throw new InvalidOperationException("Workout exercise not found.");

                entity.SessionId = workoutDto.SessionId;
                entity.ExerciseId = workoutDto.ExerciseId;
                entity.Order = workoutDto.Order;
                entity.UpdatedAt = DateTime.UtcNow;

                context.WorkoutExercises.Update(entity);

                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> DeleteWorkoutExercise(Guid id)
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var entity = await context.WorkoutExercises.FirstOrDefaultAsync(we => we.Id == id && we.DeletedAt == null);

                if (entity == null)
                    throw new InvalidOperationException("Workout exercise not found.");

                entity.DeletedAt = DateTime.UtcNow;

                context.WorkoutExercises.Update(entity);

                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<IEnumerable<WorkoutExercisesDto>> GetAllWorkoutExercisesBySessionId(Guid sessionId)
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var list = await context.WorkoutExercises.Where(we => we.SessionId == sessionId && we.DeletedAt == null).OrderBy(we => we.Order).ToListAsync();

                return _mapper.Map<IEnumerable<WorkoutExercisesDto>>(list);
            }
        }

        public async Task<WorkoutExercisesDto?> GetWorkoutExerciseById(Guid id)
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var entity = await context.WorkoutExercises.FirstOrDefaultAsync(we => we.Id == id && we.DeletedAt == null);

                return entity == null ? null : _mapper.Map<WorkoutExercisesDto>(entity);
            }
        }

        #endregion
    }
}