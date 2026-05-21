using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SetPoint.BLL._11.ExerciseSetsManagement.Dto;
using SetPoint.DAL._1.Entity;
using SetPoint.DAL._2.Context;

namespace SetPoint.BLL._11.ExerciseSetsManagement
{
    public class ExerciseSetsBll : IExerciseSetsBll
    {
        #region Fields

        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly string _connectionString;

        #endregion


        #region Constructors

        public ExerciseSetsBll(IConfiguration config)
        {
            _config = config;
            // Retrieve the connection string from configuration
            _connectionString = _config.GetConnectionString("PostgreConnection") ??
                 throw new InvalidOperationException("Connection string 'PostgreConnection' not found.");

            // Configure AutoMapper
            var conMap = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ExerciseSetsDto, ExerciseSets>();
                cfg.CreateMap<ExerciseSets, ExerciseSetsDto>();
            });
            _mapper = conMap.CreateMapper();
        }

        #endregion


        #region Methods

        public async Task<bool> SyncExerciseSet(ExerciseSetsDto dto)
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var existing = await context.ExerciseSets
                    .FirstOrDefaultAsync(s => s.Id == dto.Id ||
                                             (s.WorkoutExerciseId == dto.WorkoutExerciseId &&
                                              s.SetNumber == dto.SetNumber));

                if (existing == null)
                {
                    var entity = _mapper.Map<ExerciseSets>(dto);
                    await context.ExerciseSets.AddAsync(entity);
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
                        context.ExerciseSets.Update(existing);
                    }
                    else
                    {
                        return true;
                    }
                }
                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> CreateExerciseSet(ExerciseSetsDto setDto)
        {
            if (setDto == null)
                throw new Exception("ExerciseSetsDto cannot be null.");

            using (var context = new SetPointDbContext(_connectionString))
            {
                var exists = await context.ExerciseSets.AnyAsync(s =>
                    s.WorkoutExerciseId == setDto.WorkoutExerciseId && s.SetNumber == setDto.SetNumber && s.DeletedAt == null);

                if (exists)
                    throw new InvalidOperationException($"A set with SetNumber {setDto.SetNumber} already exists for this WorkoutExercise.");

                var entity = _mapper.Map<ExerciseSets>(setDto);

                if (entity.Id == Guid.Empty)
                    entity.Id = Guid.NewGuid();

                entity.CreatedAt = DateTime.UtcNow;

                await context.ExerciseSets.AddAsync(entity);

                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> UpdateExerciseSet(ExerciseSetsDto setDto)
        {
            if (setDto == null)
                throw new Exception("ExerciseSetsDto cannot be null.");

            using (var context = new SetPointDbContext(_connectionString))
            {
                var exists = await context.ExerciseSets.AnyAsync(s =>
                    s.WorkoutExerciseId == setDto.WorkoutExerciseId && s.SetNumber == setDto.SetNumber && s.DeletedAt == null);

                if (exists)
                    throw new InvalidOperationException($"A set with SetNumber {setDto.SetNumber} already exists for this WorkoutExercise.");

                var entity = await context.ExerciseSets.FirstOrDefaultAsync(s => s.Id == setDto.Id && s.DeletedAt == null);

                if (entity == null)
                    throw new InvalidOperationException("Exercise set not found.");

                entity.WorkoutExerciseId = setDto.WorkoutExerciseId;
                entity.SetNumber = setDto.SetNumber;
                entity.Reps = setDto.Reps;
                entity.Weight = setDto.Weight;
                entity.Rpe = setDto.Rpe;
                entity.UpdatedAt = DateTime.UtcNow;

                context.ExerciseSets.Update(entity);

                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> DeleteExerciseSet(Guid id)
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var entity = await context.ExerciseSets.FirstOrDefaultAsync(s => s.Id == id && s.DeletedAt == null);

                if (entity == null)
                    throw new InvalidOperationException("Exercise set not found.");

                entity.DeletedAt = DateTime.UtcNow;

                context.ExerciseSets.Update(entity);

                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<IEnumerable<ExerciseSetsDto>> GetAllExerciseSetsByWorkoutExerciseId(Guid workoutExerciseId)
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var list = await context.ExerciseSets.Where(s => s.WorkoutExerciseId == workoutExerciseId && s.DeletedAt == null)
                    .OrderBy(s => s.SetNumber).ToListAsync();

                return _mapper.Map<IEnumerable<ExerciseSetsDto>>(list);
            }
        }

        public async Task<ExerciseSetsDto?> GetExerciseSetById(Guid id)
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var entity = await context.ExerciseSets.FirstOrDefaultAsync(s => s.Id == id && s.DeletedAt == null);

                return entity == null ? null : _mapper.Map<ExerciseSetsDto>(entity);
            }
        }

        #endregion
    }
}