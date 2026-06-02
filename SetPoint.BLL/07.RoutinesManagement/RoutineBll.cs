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

        public async Task<bool> CloneRoutineForUserAsync(Guid routineId, Guid userId)
        {
            try
            {
                using (var context = new SetPointDbContext(_connectionString))
                {
                    var existing = await context.Routines.AsNoTracking().FirstOrDefaultAsync(r => r.Id == routineId);

                    if (existing == null) return false;

                    var dateNow = DateTime.UtcNow;

                    var cloneRoutine = _mapper.Map<Routines>(existing);
                    cloneRoutine.Id = Guid.NewGuid();
                    cloneRoutine.UserId = userId;
                    cloneRoutine.CreatedAt = dateNow;
                    cloneRoutine.UpdatedAt = dateNow;

                    await context.Routines.AddAsync(cloneRoutine);

                    var exercises = await context.RoutineExercises.AsNoTracking().Where(re => re.RoutineId == routineId).ToListAsync();

                    foreach (var exercise in exercises)
                    {
                        var cloneExercise = _mapper.Map<RoutineExercises>(exercise);
                        cloneExercise.Id = Guid.NewGuid();
                        cloneExercise.RoutineId = cloneRoutine.Id;
                        cloneExercise.CreatedAt = dateNow;
                        cloneExercise.UpdatedAt = dateNow;
                        await context.RoutineExercises.AddAsync(cloneExercise);
                    }

                    return await context.SaveChangesAsync() > 0;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
    }
}
