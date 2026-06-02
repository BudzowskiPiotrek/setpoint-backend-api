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
        #endregion
    }
}