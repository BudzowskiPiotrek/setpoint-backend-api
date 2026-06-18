using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SetPoint.BLL._08.RoutineExercisesManagement.Dto;
using SetPoint.DAL._1.Entity;
using SetPoint.DAL._2.Context;

namespace SetPoint.BLL._08.RoutineExercisesManagement
{
    public class RoutineExercisesBll : IRoutineExercisesBll
    {
        #region Fields
        private readonly IMapper _mapper;
        private readonly SetPointDbContext _context;
        #endregion


        #region Constructors
        public RoutineExercisesBll(SetPointDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        #endregion


        #region Methods
        public async Task<bool> SyncRoutineExercise(RoutineExerciseDto dto)
        {
            var existing = await _context.RoutineExercises
                .FirstOrDefaultAsync(re => re.Id == dto.Id ||
                                          (re.RoutineId == dto.RoutineId &&
                                           re.ExerciseId == dto.ExerciseId &&
                                           re.Order == dto.Order));

            if (existing == null)
            {
                var entity = _mapper.Map<RoutineExercises>(dto);
                await _context.RoutineExercises.AddAsync(entity);
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
                    _context.RoutineExercises.Update(existing);
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