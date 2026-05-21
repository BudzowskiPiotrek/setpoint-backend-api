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

        public async Task<bool> CreateSession(WorkoutSessionsDto workoutDto)
        {
            if (workoutDto == null)
                throw new Exception("WorkoutSessionsDto cannot be null.");

            using (var context = new SetPointDbContext(_connectionString))
            {
                var exists = await context.WorkoutSessions.AnyAsync(w => w.UserId == workoutDto.UserId && w.Date == workoutDto.Date && w.DeletedAt == null);

                if (exists)
                    throw new InvalidOperationException("A workout session for this user on the specified date already exists.");

                var entity = _mapper.Map<WorkoutSessions>(workoutDto);

                if (entity.Id == Guid.Empty)
                    entity.Id = Guid.NewGuid();

                entity.CreatedAt = DateTime.UtcNow;

                var log = new Logs
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    UserId = entity.UserId,
                    Type = $"Started workout session at {entity.CreatedAt}"
                };

                await context.Logs.AddAsync(log);
                await context.WorkoutSessions.AddAsync(entity);

                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> UpdateSession(WorkoutSessionsDto workoutDto)
        {
            if (workoutDto == null)
                throw new Exception("WorkoutSessionsDto cannot be null.");

            using (var context = new SetPointDbContext(_connectionString))
            {
                var exists = await context.WorkoutSessions.AnyAsync(w => w.UserId == workoutDto.UserId && w.Date == workoutDto.Date && w.DeletedAt == null);

                if (exists)
                    throw new InvalidOperationException("A workout session for this user on the specified date already exists.");

                var entity = await context.WorkoutSessions.FirstOrDefaultAsync(w => w.Id == workoutDto.Id && w.DeletedAt == null);

                if (entity == null)
                    throw new InvalidOperationException("Workout session not found.");

                var log = new Logs
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    UserId = entity.UserId,
                    Type = $"OLD DATA BEFORE UPDATE: Duration: {entity.Date}, Date: {entity.DurationMinutes}, Notes: {entity.Notes}"
                };

                entity.Date = workoutDto.Date;
                entity.DurationMinutes = workoutDto.DurationMinutes;
                entity.Notes = workoutDto.Notes;
                entity.UpdatedAt = DateTime.UtcNow;

                await context.Logs.AddAsync(log);
                context.WorkoutSessions.Update(entity);

                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> DeleteSession(Guid id)
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var entity = await context.WorkoutSessions.FirstOrDefaultAsync(w => w.Id == id && w.DeletedAt == null);

                if (entity == null)
                    throw new InvalidOperationException("Workout session not found.");

                entity.DeletedAt = DateTime.UtcNow;

                var log = new Logs
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    UserId = entity.UserId,
                    Type = $"Deleted workout session from date: {entity.Date}"
                };

                await context.Logs.AddAsync(log);
                context.WorkoutSessions.Update(entity);

                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<IEnumerable<WorkoutSessionsDto>> GetAllPersonalSessions(Guid userId)
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var sessions = await context.WorkoutSessions.Where(w => w.UserId == userId && w.DeletedAt == null).OrderByDescending(w => w.Date).ToListAsync();

                return _mapper.Map<IEnumerable<WorkoutSessionsDto>>(sessions);
            }
        }

        public async Task<WorkoutSessionsDto?> GetSessionById(Guid id)
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var entity = await context.WorkoutSessions.FirstOrDefaultAsync(w => w.Id == id && w.DeletedAt == null);

                return entity == null ? null : _mapper.Map<WorkoutSessionsDto>(entity);
            }
        }

        #endregion
    }
}