using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SetPoint.BLL._06.ExerciseMuscleManagement.Dto;
using SetPoint.DAL._1.Entity;
using SetPoint.DAL._2.Context;

namespace SetPoint.BLL._06.ExerciseMuscleManagement
{
    public class ExerciseMuscleGroupBll : IExerciseMuscleGroupBll
    {
        #region Fields

        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly string _connectionString;

        #endregion


        #region Constructors

        public ExerciseMuscleGroupBll(IConfiguration config)
        {
            _config = config;
            // Retrieve the connection string from configuration
            _connectionString = _config.GetConnectionString("PostgreConnection") ??
                 throw new InvalidOperationException("Connection string 'PostgreConnection' not found.");

            // Configure AutoMapper
            var conMap = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ExerciseMuscleDto, ExerciseMuscleGroup>();
                cfg.CreateMap<ExerciseMuscleGroup, ExerciseMuscleDto>();
            });
            _mapper = conMap.CreateMapper();
        }

        #endregion


        #region Methods

        public async Task<bool> SyncExerciseMuscleGroup(ExerciseMuscleDto dto)
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var existing = await context.ExerciseMuscleGroups
                    .FirstOrDefaultAsync(em => em.Id == dto.Id ||
                                              (em.ExerciseId == dto.ExerciseId && em.MuscleId == dto.MuscleId));
                if (existing == null)
                {
                    var entity = _mapper.Map<ExerciseMuscleGroup>(dto);
                    await context.ExerciseMuscleGroups.AddAsync(entity);
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
                        context.ExerciseMuscleGroups.Update(existing);
                    }
                    else
                    {
                        return true;
                    }
                }
                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> CreateExerciseMuscleGroup(ExerciseMuscleDto exerciseDto)
        {
            if (exerciseDto == null)
                throw new Exception("Exercise-Muscle cannot be null.");

            using (var context = new SetPointDbContext(_connectionString))
            {
                var exists = await context.ExerciseMuscleGroups.AnyAsync(em =>
                    em.ExerciseId == exerciseDto.ExerciseId && em.MuscleId == exerciseDto.MuscleId && em.DeletedAt == null);

                if (exists)
                    throw new InvalidOperationException("This exercise already has this muscle group assigned.");

                var entity = _mapper.Map<ExerciseMuscleGroup>(exerciseDto);

                if (entity.Id == Guid.Empty)
                    entity.Id = Guid.NewGuid();

                entity.CreatedAt = DateTime.UtcNow;

                await context.ExerciseMuscleGroups.AddAsync(entity);

                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> UpdateExerciseMuscleGroup(ExerciseMuscleDto exerciseDto)
        {
            if (exerciseDto == null)
                throw new Exception("ExerciseMuscleDto cannot be null.");

            using (var context = new SetPointDbContext(_connectionString))
            {
                var exists = await context.ExerciseMuscleGroups.AnyAsync(em =>
                    em.ExerciseId == exerciseDto.ExerciseId && em.MuscleId == exerciseDto.MuscleId && em.DeletedAt == null);

                if (exists)
                    throw new InvalidOperationException("This exercise already has this muscle group assigned.");

                var entity = await context.ExerciseMuscleGroups.FirstOrDefaultAsync(em => em.Id == exerciseDto.Id && em.DeletedAt == null);

                if (entity == null)
                    throw new InvalidOperationException("Relationship not found.");

                entity.ExerciseId = exerciseDto.ExerciseId;
                entity.MuscleId = exerciseDto.MuscleId;
                entity.UpdatedAt = DateTime.UtcNow;

                context.ExerciseMuscleGroups.Update(entity);

                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> DeleteExerciseMuscleGroup(Guid id)
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var entity = await context.ExerciseMuscleGroups.FirstOrDefaultAsync(em => em.Id == id && em.DeletedAt == null);

                if (entity == null)
                    throw new InvalidOperationException("Relationship not found.");

                entity.DeletedAt = DateTime.UtcNow;

                context.ExerciseMuscleGroups.Update(entity);

                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<IEnumerable<ExerciseMuscleDto>> GetAllExerciseMuscleGroups(Guid exerciseId)
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var list = await context.ExerciseMuscleGroups.Where(em => em.ExerciseId == exerciseId && em.DeletedAt == null).ToListAsync();

                return _mapper.Map<IEnumerable<ExerciseMuscleDto>>(list);
            }
        }

        public async Task<ExerciseMuscleDto?> GetExerciseMuscleGroupById(Guid id)
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var entity = await context.ExerciseMuscleGroups.FirstOrDefaultAsync(em => em.Id == id && em.DeletedAt == null);

                return entity == null ? null : _mapper.Map<ExerciseMuscleDto>(entity);
            }
        }
        #endregion
    }
}
