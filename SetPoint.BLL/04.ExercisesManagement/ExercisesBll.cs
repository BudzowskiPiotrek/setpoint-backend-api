using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SetPoint.BLL._04.ExercisesManagement.Dto;
using SetPoint.DAL._1.Entity;
using SetPoint.DAL._2.Context;

namespace SetPoint.BLL._04.ExercisesManagement
{
    public class ExercisesBll : IExercisesBll
    {
        #region Fields

        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly string _connectionString;

        #endregion


        #region Constructors

        public ExercisesBll(IConfiguration config)
        {
            _config = config;
            // Retrieve the connection string from configuration
            _connectionString = _config.GetConnectionString("PostgreConnection") ??
                 throw new InvalidOperationException("Connection string 'PostgreConnection' not found.");

            // Configure AutoMapper
            var conMap = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ExercisesDto, Exercise>();
                cfg.CreateMap<Exercise, ExercisesDto>();
            });
            _mapper = conMap.CreateMapper();
        }
        #endregion

        #region Methods

        public async Task<bool> SyncExercise(ExercisesDto dto)
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var existing = await context.Exercises
                    .FirstOrDefaultAsync(e => e.Id == dto.Id || e.Name.ToLower() == dto.Name.ToLower());

                if (existing == null)
                {
                    var entity = _mapper.Map<Exercise>(dto);
                    await context.Exercises.AddAsync(entity);
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
                        context.Exercises.Update(existing);
                    }
                    else
                    {
                        return true;
                    }
                }
                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> CreateExercise(ExercisesDto exerciseDto)
        {
            if (exerciseDto == null)
                throw new Exception("ExercisesDto cannot be null.");

            using (var context = new SetPointDbContext(_connectionString))
            {
                var exists = await context.Exercises.AnyAsync(e => e.Name == exerciseDto.Name && e.DeletedAt == null);

                if (exists)
                    throw new InvalidOperationException("An exercise with this name already exists.");

                var exerciseEntity = _mapper.Map<Exercise>(exerciseDto);

                if (exerciseEntity.Id == Guid.Empty)
                {
                    exerciseEntity.Id = Guid.NewGuid();
                }
                exerciseEntity.CreatedAt = DateTime.UtcNow;

                await context.Exercises.AddAsync(exerciseEntity);

                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> UpdateExercise(ExercisesDto exerciseDto)
        {
            if (exerciseDto == null)
                throw new Exception("ExercisesDto cannot be null.");

            using (var context = new SetPointDbContext(_connectionString))
            {
                var exerciseEntity = await context.Exercises.FirstOrDefaultAsync(e => e.Id == exerciseDto.Id && e.DeletedAt == null);

                if (exerciseEntity == null)
                    throw new InvalidOperationException("Exercise not found.");

                if (!String.IsNullOrWhiteSpace(exerciseDto.Name))
                    exerciseEntity.Name = exerciseDto.Name;

                if (!String.IsNullOrWhiteSpace(exerciseDto.Description))
                    exerciseEntity.Description = exerciseDto.Description;

                exerciseEntity.ImageUrl = exerciseDto.ImageUrl;
                exerciseEntity.EquipmentType = exerciseDto.EquipmentType;
                exerciseEntity.UpdatedAt = DateTime.UtcNow;

                context.Exercises.Update(exerciseEntity);

                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> DeleteExercise(Guid id)
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var exerciseEntity = await context.Exercises.FirstOrDefaultAsync(e => e.Id == id && e.DeletedAt == null);

                if (exerciseEntity == null)
                    throw new InvalidOperationException("Exercise not found.");

                exerciseEntity.DeletedAt = DateTime.UtcNow;

                context.Exercises.Update(exerciseEntity);

                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<IEnumerable<ExercisesDto>> GetAllExercises()
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var exercises = await context.Exercises.Where(e => e.DeletedAt == null).ToListAsync();

                return _mapper.Map<IEnumerable<ExercisesDto>>(exercises);
            }
        }

        public async Task<ExercisesDto?> GetExerciseById(Guid id)
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var exerciseEntity = await context.Exercises
                    .FirstOrDefaultAsync(e => e.Id == id && e.DeletedAt == null);

                return exerciseEntity == null ? null : _mapper.Map<ExercisesDto>(exerciseEntity);
            }
        }

        #endregion
    }
}