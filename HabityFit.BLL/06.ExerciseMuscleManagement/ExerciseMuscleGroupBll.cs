using AutoMapper;
using Microsoft.EntityFrameworkCore;
using HabityFit.BLL._06.ExerciseMuscleManagement.Dto;
using HabityFit.DAL._1.Entity;
using HabityFit.DAL._2.Context;

namespace HabityFit.BLL._06.ExerciseMuscleManagement
{
    public class ExerciseMuscleGroupBll : IExerciseMuscleGroupBll
    {
        #region Fields
        private readonly IMapper _mapper;
        private readonly HabityFitDbContext _context;
        #endregion


        #region Constructors
        public ExerciseMuscleGroupBll(HabityFitDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
                var existingId = existing.Id;
                if (existing.DeletedAt != null && dto.DeletedAt == null)
                {
                    existing.DeletedAt = null;
                }
                if (dto.UpdatedAt > existing.UpdatedAt || existing.UpdatedAt == null)
                {
                    _mapper.Map(dto, existing);
                    existing.Id = existingId;
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
