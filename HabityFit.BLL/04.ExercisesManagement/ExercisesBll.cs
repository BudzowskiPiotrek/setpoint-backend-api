using AutoMapper;
using Microsoft.EntityFrameworkCore;
using HabityFit.BLL._04.ExercisesManagement.Dto;
using HabityFit.DAL._1.Entity;
using HabityFit.DAL._2.Context;

namespace HabityFit.BLL._04.ExercisesManagement
{
    public class ExercisesBll : IExercisesBll
    {
        #region Fields              
        private readonly IMapper _mapper;
        private readonly HabityFitDbContext _context;
        #endregion


        #region Constructors
        public ExercisesBll(HabityFitDbContext context, IMapper mapper)
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
                var existingId = existing.Id;
                if (existing.DeletedAt != null && dto.DeletedAt == null)
                {
                    existing.DeletedAt = null;
                }

                if (dto.UpdatedAt > existing.UpdatedAt || existing.UpdatedAt == null)
                {
                    _mapper.Map(dto, existing);
                    existing.Id = existingId;
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