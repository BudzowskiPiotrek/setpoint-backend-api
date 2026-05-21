using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SetPoint.BLL._08.RoutineExercisesManagement.Dto;
using SetPoint.DAL._1.Entity;
using SetPoint.DAL._2.Context;

namespace SetPoint.BLL._08.RoutineExercisesManagement
{
    public class RoutineExercisesBll : IRoutineExercisesBll
    {
        #region Fields

        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly string _connectionString;

        #endregion


        #region Constructors

        public RoutineExercisesBll(IConfiguration config)
        {
            _config = config;
            // Retrieve the connection string from configuration
            _connectionString = _config.GetConnectionString("PostgreConnection") ??
                 throw new InvalidOperationException("Connection string 'PostgreConnection' not found.");

            // Configure AutoMapper
            var conMap = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RoutineExerciseDto, RoutineExercises>();
                cfg.CreateMap<RoutineExercises, RoutineExerciseDto>();
            });
            _mapper = conMap.CreateMapper();
        }

        #endregion


        #region Methods

        public async Task<bool> SyncRoutineExercise(RoutineExerciseDto dto)
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var existing = await context.RoutineExercises
                    .FirstOrDefaultAsync(re => re.Id == dto.Id ||
                                              (re.RoutineId == dto.RoutineId &&
                                               re.ExerciseId == dto.ExerciseId &&
                                               re.Order == dto.Order));

                if (existing == null)
                {
                    var entity = _mapper.Map<RoutineExercises>(dto);
                    await context.RoutineExercises.AddAsync(entity);
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
                        context.RoutineExercises.Update(existing);
                    }
                    else
                    {
                        return true;
                    }
                }
                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> CreateRoutineExercise(RoutineExerciseDto routineDto)
        {
            if (routineDto == null)
                throw new Exception("RoutineExerciseDto cannot be null.");

            using (var context = new SetPointDbContext(_connectionString))
            {
                var exists = await context.RoutineExercises.AnyAsync(r =>
                    r.ExerciseId == routineDto.ExerciseId && r.RoutineId == routineDto.RoutineId && r.Order == routineDto.Order && r.DeletedAt == null);

                if (exists)
                    throw new InvalidCastException("A routine exercise relationship with the same exercise, routine and order already exists.");

                var entity = _mapper.Map<RoutineExercises>(routineDto);

                if (entity.Id == Guid.Empty)
                    entity.Id = Guid.NewGuid();

                entity.CreatedAt = DateTime.UtcNow;

                await context.RoutineExercises.AddAsync(entity);

                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> UpdateRoutineExercise(RoutineExerciseDto routineDto)
        {
            if (routineDto == null)
                throw new Exception("RoutineExerciseDto cannot be null.");

            using (var context = new SetPointDbContext(_connectionString))
            {
                var exists = await context.RoutineExercises.AnyAsync(r =>
                    r.ExerciseId == routineDto.ExerciseId && r.RoutineId == routineDto.RoutineId && r.Order == routineDto.Order && r.DeletedAt == null);

                if (exists)
                    throw new InvalidCastException("A routine exercise relationship with the same exercise, routine and order already exists.");

                var entity = await context.RoutineExercises.FirstOrDefaultAsync(re => re.Id == routineDto.Id && re.DeletedAt == null);

                if (entity == null)
                    throw new InvalidOperationException("Routine exercise relationship not found.");

                entity.RoutineId = routineDto.RoutineId;
                entity.ExerciseId = routineDto.ExerciseId;
                entity.Order = routineDto.Order;
                entity.Sets = routineDto.Sets;
                entity.Reps = routineDto.Reps;
                entity.TargetWeight = routineDto.TargetWeight;
                entity.RestSecond = routineDto.RestSecond;
                entity.UpdatedAt = DateTime.UtcNow;

                context.RoutineExercises.Update(entity);

                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> DeleteRoutineExercise(Guid id)
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var entity = await context.RoutineExercises.FirstOrDefaultAsync(re => re.Id == id && re.DeletedAt == null);

                if (entity == null)
                    throw new InvalidOperationException("Routine exercise relationship not found.");

                entity.DeletedAt = DateTime.UtcNow;

                context.RoutineExercises.Update(entity);

                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<IEnumerable<RoutineExerciseDto>> GetAllRoutineExercises(Guid routineId)
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var list = await context.RoutineExercises.Where(re => re.RoutineId == routineId && re.DeletedAt == null).OrderBy(re => re.Order).ToListAsync();

                return _mapper.Map<IEnumerable<RoutineExerciseDto>>(list);
            }
        }

        public async Task<RoutineExerciseDto?> GetRoutineExerciseById(Guid id)
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var entity = await context.RoutineExercises.FirstOrDefaultAsync(re => re.Id == id && re.DeletedAt == null);

                return entity == null ? null : _mapper.Map<RoutineExerciseDto>(entity);
            }
        }

        #endregion
    }
}