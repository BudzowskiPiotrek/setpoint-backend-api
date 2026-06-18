using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SetPoint.BLL._05.MuscleGroupsManagement.Dto;
using SetPoint.DAL._1.Entity;
using SetPoint.DAL._2.Context;

namespace SetPoint.BLL._05.MuscleGroupsManagement
{
    public class MuscleGroupBll : IMuscleGroupBll
    {
        #region Fields
        private readonly IMapper _mapper;
        private readonly SetPointDbContext _context;
        #endregion


        #region Constructors
        public MuscleGroupBll(SetPointDbContext context)
        {
            _context = context;
            // Configure AutoMapper
            var conMap = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<MuscleGroupDto, MuscleGroup>();
                cfg.CreateMap<MuscleGroup, MuscleGroupDto>();
            });
            _mapper = conMap.CreateMapper();
        }
        #endregion


        #region Methods
        public async Task<bool> SyncMuscleGroup(MuscleGroupDto dto)
        {
            var existing = await _context.MuscleGroups
                .FirstOrDefaultAsync(m => m.Id == dto.Id || m.Name.ToLower() == dto.Name.ToLower());

            if (existing == null)
            {
                var entity = _mapper.Map<MuscleGroup>(dto);
                await _context.MuscleGroups.AddAsync(entity);
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
                    _context.MuscleGroups.Update(existing);
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
