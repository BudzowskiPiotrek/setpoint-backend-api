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
        #endregion
    }
}