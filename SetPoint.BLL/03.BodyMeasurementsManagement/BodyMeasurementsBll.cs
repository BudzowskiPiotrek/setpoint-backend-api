using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SetPoint.BLL._03.BodyMeasurementsManagement.Dto;
using SetPoint.DAL._1.Entity;
using SetPoint.DAL._2.Context;

namespace SetPoint.BLL._03.BodyMeasurementsManagement
{
    public class BodyMeasurementsBll : IBodyMeasurementsBll
    {
        #region Fields

        private readonly IConfiguration _config;

        private readonly IMapper _mapper;

        private readonly string _connectionString;

        #endregion


        #region Constructors

        public BodyMeasurementsBll(IConfiguration config)
        {
            _config = config;
            // Retrieve the connection string from configuration
            _connectionString = _config.GetConnectionString("PostgreConnection") ??
                 throw new InvalidOperationException("Connection string 'PostgreConnection' not found.");

            // Configure AutoMapper
            var conMap = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<BodyMeasurementsDto, BodyMeasurements>();
                cfg.CreateMap<BodyMeasurements, BodyMeasurementsDto>();
            });
            _mapper = conMap.CreateMapper();
        }
        #endregion


        #region Methods

        public async Task<bool> SyncBody(BodyMeasurementsDto dto)
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var existing = await context.BodyMeasurements
                    .FirstOrDefaultAsync(b => b.Id == dto.Id || (b.IdUser == dto.IdUser && b.Date == dto.Date));

                if (existing == null)
                {
                    var entity = _mapper.Map<BodyMeasurements>(dto);
                    await context.BodyMeasurements.AddAsync(entity);
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
                        context.BodyMeasurements.Update(existing);
                    }
                    else
                    {
                        return true;
                    }
                }
                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> CreateBody(BodyMeasurementsDto bodyDto)
        {
            if (bodyDto == null)
                throw new Exception("BodyMeasurementsDto cannot be null.");

            using (var context = new SetPointDbContext(_connectionString))
            {
                var exists = await context.BodyMeasurements.AnyAsync(b => b.IdUser == bodyDto.IdUser && b.Date == bodyDto.Date);

                if (exists)
                    throw new Exception("A body measurement for this user in this date already exists.");

                var bodyEntity = _mapper.Map<BodyMeasurements>(bodyDto);

                if (bodyEntity.Id == Guid.Empty)
                {
                    bodyEntity.Id = Guid.NewGuid();
                }
                bodyEntity.CreatedAt = DateTime.UtcNow;

                var log = new Logs
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    UserId = bodyEntity.IdUser,
                    Type = $"Created body measurement on {bodyDto.Date}"
                };

                await context.Logs.AddAsync(log);
                await context.BodyMeasurements.AddAsync(bodyEntity);

                return await context.SaveChangesAsync() > 0;
            }
        }
        public async Task<bool> UpdateBody(BodyMeasurementsDto bodyDto)
        {
            if (bodyDto == null)
                throw new Exception("BodyMeasurementsDto cannot be null.");

            using (var context = new SetPointDbContext(_connectionString))
            {
                var bodyEntity = await context.BodyMeasurements.FirstOrDefaultAsync(
                    u => u.Id == bodyDto.Id && u.Date == bodyDto.Date && u.DeletedAt == null);

                if (bodyEntity == null)
                    throw new InvalidOperationException("Body measurements not found");

                var log = new Logs
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    UserId = bodyEntity.IdUser,
                    Type = $"OLD DATA BEFORE UPDATE: Weight: {bodyEntity.Weight}, Muscle: {bodyEntity.MuscleMass}, Fat: {bodyEntity.FatMass}, Water: {bodyEntity.BodyWater}"
                };

                bodyEntity.Weight = bodyDto.Weight;
                bodyEntity.MuscleMass = bodyDto.MuscleMass;
                bodyEntity.FatMass = bodyDto.FatMass;
                bodyEntity.BodyWater = bodyDto.BodyWater;
                bodyEntity.UpdatedAt = DateTime.UtcNow;

                await context.Logs.AddAsync(log);
                context.BodyMeasurements.Update(bodyEntity);

                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> DeleteBody(Guid bodyId)
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var bodyEntity = await context.BodyMeasurements.FirstOrDefaultAsync(u => u.Id == bodyId && u.DeletedAt == null);

                if (bodyEntity == null)
                    throw new InvalidOperationException("Body measurements not found");

                bodyEntity.DeletedAt = DateTime.UtcNow;
                var log = new Logs
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    UserId = bodyEntity.IdUser,
                    Type = $"Deleted body measurement for user."
                };

                await context.Logs.AddAsync(log);
                context.BodyMeasurements.Update(bodyEntity);

                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<IEnumerable<BodyMeasurementsDto>> GetAllBodyByUserId(Guid userId)
        {
            using (var context = new SetPointDbContext(_connectionString))
            {

                var bodys = await context.BodyMeasurements.Where(u => u.IdUser == userId && u.DeletedAt == null).ToListAsync();

                return _mapper.Map<IEnumerable<BodyMeasurementsDto>>(bodys);

            }
        }

        public async Task<BodyMeasurementsDto?> GetBodyById(Guid bodyId)
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var bodyEntity = await context.BodyMeasurements.FirstOrDefaultAsync(u => u.Id == bodyId && u.DeletedAt == null);

                return bodyEntity == null ? null : _mapper.Map<BodyMeasurementsDto>(bodyEntity);
            }
        }

        #endregion
    }
}
