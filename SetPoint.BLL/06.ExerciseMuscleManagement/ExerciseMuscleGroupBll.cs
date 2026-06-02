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
        #endregion
    }
}
