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

        public async Task<bool> CreateMuscleGroup(MuscleGroupDto muscleGroupDto)
        {
            if (muscleGroupDto == null)
                throw new Exception("Muscle group cannot be null.");

            using (var context = new SetPointDbContext(_connectionString))
            {
                var exists = await context.MuscleGroups.AnyAsync(m => m.Name == muscleGroupDto.Name && m.DeletedAt == null);

                if (exists)
                    throw new InvalidOperationException("A muscle group with this name already exists.");

                var muscleGroupEntity = _mapper.Map<MuscleGroup>(muscleGroupDto);

                if (muscleGroupEntity.Id == Guid.Empty)
                    muscleGroupEntity.Id = Guid.NewGuid();

                muscleGroupEntity.CreatedAt = DateTime.UtcNow;

                await context.MuscleGroups.AddAsync(muscleGroupEntity);

                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> UpdateMuscleGroup(MuscleGroupDto muscleGroupDto)
        {
            if (muscleGroupDto == null)
                throw new Exception("Muscle group cannot be null.");

            using (var context = new SetPointDbContext(_connectionString))
            {
                var muscleEntity = await context.MuscleGroups.FirstOrDefaultAsync(m => m.Id == muscleGroupDto.Id && m.DeletedAt == null);

                if (muscleEntity == null)
                    throw new InvalidOperationException("Muscle group not found.");

                if (!string.IsNullOrWhiteSpace(muscleGroupDto.Name))
                    muscleEntity.Name = muscleGroupDto.Name;

                muscleEntity.Description = muscleGroupDto.Description;
                muscleEntity.UpdatedAt = DateTime.UtcNow;

                context.MuscleGroups.Update(muscleEntity);

                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> DeleteMuscleGroup(Guid id)
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var muscleGroupEntity = await context.MuscleGroups.FirstOrDefaultAsync(m => m.Id == id && m.DeletedAt == null);

                if (muscleGroupEntity == null)
                    throw new InvalidOperationException("Muscle group not found.");

                muscleGroupEntity.DeletedAt = DateTime.UtcNow;

                context.MuscleGroups.Update(muscleGroupEntity);

                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<IEnumerable<MuscleGroupDto>> GetAllMuscleGroups()
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var groups = await context.MuscleGroups.Where(m => m.DeletedAt == null).ToListAsync();

                return _mapper.Map<IEnumerable<MuscleGroupDto>>(groups);
            }
        }

        public async Task<MuscleGroupDto?> GetMuscleGroupById(Guid id)
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var muscleGroupEntity = await context.MuscleGroups.FirstOrDefaultAsync(m => m.Id == id && m.DeletedAt == null);

                return muscleGroupEntity == null ? null : _mapper.Map<MuscleGroupDto>(muscleGroupEntity);
            }
        }

        #endregion
    }
}
