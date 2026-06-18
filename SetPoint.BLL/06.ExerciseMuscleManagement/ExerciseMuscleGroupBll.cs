using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SetPoint.BLL._06.ExerciseMuscleManagement.Dto;
using SetPoint.DAL._1.Entity;
using SetPoint.DAL._2.Context;

namespace SetPoint.BLL._06.ExerciseMuscleManagement
{
    public class ExerciseMuscleGroupBll : IExerciseMuscleGroupBll
    {
        #region Fields
        private readonly IMapper _mapper;
        private readonly SetPointDbContext _context;
        #endregion


        #region Constructors
        public ExerciseMuscleGroupBll(SetPointDbContext context)
        {
            _context = context;

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
            var existing = await _context.ExerciseMuscleGroups
                .FirstOrDefaultAsync(em => em.Id == dto.Id ||
                                          (em.ExerciseId == dto.ExerciseId && em.MuscleId == dto.MuscleId));
            if (existing == null)
            {
                var entity = _mapper.Map<ExerciseMuscleGroup>(dto);
                await _context.ExerciseMuscleGroups.AddAsync(entity);
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
                    _context.ExerciseMuscleGroups.Update(existing);
                }
                else
                {
                    return true;
                }
            }
            return await _context.SaveChangesAsync() > 0;
        }
        #endregion
    }
}
