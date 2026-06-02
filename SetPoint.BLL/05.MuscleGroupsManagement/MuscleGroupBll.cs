using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SetPoint.BLL._05.MuscleGroupsManagement.Dto;
using SetPoint.DAL._1.Entity;
using SetPoint.DAL._2.Context;

namespace SetPoint.BLL._05.MuscleGroupsManagement
{
    public class MuscleGroupBll : IMuscleGroupBll
    {
        #region Fields

        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly string _connectionString;

        #endregion


        #region Constructors

        public MuscleGroupBll(IConfiguration config)
        {
            _config = config;
            // Retrieve the connection string from configuration
            _connectionString = _config.GetConnectionString("PostgreConnection") ??
                 throw new InvalidOperationException("Connection string 'PostgreConnection' not found.");

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
            using (var context = new SetPointDbContext(_connectionString))
            {
                var existing = await context.MuscleGroups
                    .FirstOrDefaultAsync(m => m.Id == dto.Id || m.Name.ToLower() == dto.Name.ToLower());

                if (existing == null)
                {
                    var entity = _mapper.Map<MuscleGroup>(dto);
                    await context.MuscleGroups.AddAsync(entity);
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
                        context.MuscleGroups.Update(existing);
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
