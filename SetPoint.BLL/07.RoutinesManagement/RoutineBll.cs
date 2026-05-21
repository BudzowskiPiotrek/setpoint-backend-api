using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SetPoint.BLL._07.RoutinesManagement.Dto;
using SetPoint.DAL._1.Entity;
using SetPoint.DAL._2.Context;

namespace SetPoint.BLL._07.RoutinesManagement
{
    public class RoutineBll : IRoutineBll
    {
        #region Fields

        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly string _connectionString;

        #endregion


        #region Constructors

        public RoutineBll(IConfiguration config)
        {
            _config = config;
            // Retrieve the connection string from configuration
            _connectionString = _config.GetConnectionString("PostgreConnection") ??
                 throw new InvalidOperationException("Connection string 'PostgreConnection' not found.");
            // Configure AutoMapper
            var conMap = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RoutineDto, Routines>();
                cfg.CreateMap<Routines, RoutineDto>();
            });
            _mapper = conMap.CreateMapper();
        }

        #endregion


        #region Methods

        public async Task<bool> SyncRoutine(RoutineDto dto)
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var existing = await context.Routines
                    .FirstOrDefaultAsync(r => r.Id == dto.Id ||
                                             (r.UserId == dto.UserId && r.Name.ToLower() == dto.Name.ToLower()));

                if (existing == null)
                {
                    var entity = _mapper.Map<Routines>(dto);
                    await context.Routines.AddAsync(entity);
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
                        context.Routines.Update(existing);
                    }
                    else
                    {
                        return true;
                    }
                }
                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> CreateRoutine(RoutineDto routineDto)
        {
            if (routineDto == null)
                throw new Exception("RoutineDto cannot be null.");

            using (var context = new SetPointDbContext(_connectionString))
            {

                var routineEntity = _mapper.Map<Routines>(routineDto);

                if (routineEntity.Id == Guid.Empty)
                    routineEntity.Id = Guid.NewGuid();

                routineEntity.CreatedAt = DateTime.UtcNow;

                var log = new Logs
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    UserId = routineEntity.UserId,
                    Type = $"Created routine: {routineDto.Name}"
                };

                await context.Logs.AddAsync(log);
                await context.Routines.AddAsync(routineEntity);

                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> UpdateRoutine(RoutineDto routineDto)
        {
            if (routineDto == null)
                throw new Exception("RoutineDto cannot be null.");

            using (var context = new SetPointDbContext(_connectionString))
            {
                var routineEntity = await context.Routines.FirstOrDefaultAsync(r => r.Id == routineDto.Id && r.DeletedAt == null);

                if (routineEntity == null)
                    throw new InvalidOperationException("Routine not found.");

                var log = new Logs
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    UserId = routineEntity.UserId,
                    Type = $"OLD DATA BEFORE UPDATE: Name: {routineEntity.Name}, Description: {routineEntity.Description}"
                };

                if (!string.IsNullOrWhiteSpace(routineDto.Name))
                    routineEntity.Name = routineDto.Name;

                routineEntity.Description = routineDto.Description;
                routineEntity.UpdatedAt = DateTime.UtcNow;

                await context.Logs.AddAsync(log);
                context.Routines.Update(routineEntity);

                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> DeleteRoutine(Guid id)
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var routineEntity = await context.Routines.FirstOrDefaultAsync(r => r.Id == id && r.DeletedAt == null);

                if (routineEntity == null)
                    throw new InvalidOperationException("Routine not found.");

                routineEntity.DeletedAt = DateTime.UtcNow;

                var log = new Logs
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    UserId = routineEntity.UserId,
                    Type = $"Deleted routine: {routineEntity.Name}"
                };

                await context.Logs.AddAsync(log);
                context.Routines.Update(routineEntity);

                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<IEnumerable<RoutineDto>> GetAllPersonalRoutines(Guid userId)
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var routines = await context.Routines.Where(r => r.UserId == userId && r.DeletedAt == null).ToListAsync();

                return _mapper.Map<IEnumerable<RoutineDto>>(routines);
            }
        }

        public async Task<IEnumerable<RoutineDto>> GetAllRoutines()
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var routines = await context.Routines.Where(r => r.DeletedAt == null).ToListAsync();

                return _mapper.Map<IEnumerable<RoutineDto>>(routines);
            }
        }

        public async Task<RoutineDto?> GetRoutineById(Guid id)
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var routineEntity = await context.Routines.FirstOrDefaultAsync(r => r.Id == id && r.DeletedAt == null);

                return routineEntity == null ? null : _mapper.Map<RoutineDto>(routineEntity);
            }
        }
        #endregion
    }
}
