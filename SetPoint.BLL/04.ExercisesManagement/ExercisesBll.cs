using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SetPoint.BLL._04.ExercisesManagement.Dto;
using SetPoint.DAL._1.Entity;
using SetPoint.DAL._2.Context;

namespace SetPoint.BLL._04.ExercisesManagement
{
    public class ExercisesBll : IExercisesBll
    {
        #region Fields              
        private readonly IMapper _mapper;
        private readonly SetPointDbContext _context;
        #endregion


        #region Constructors
        public ExercisesBll(SetPointDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        #endregion


        #region Methods
        public async Task<bool> SyncExercise(ExercisesDto dto)
        {
            var existing = await _context.Exercises
                .FirstOrDefaultAsync(e => e.Id == dto.Id || e.Name.ToLower() == dto.Name.ToLower());

            if (existing == null)
            {
                var entity = _mapper.Map<Exercise>(dto);
                await _context.Exercises.AddAsync(entity);
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
                    _context.Exercises.Update(existing);
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