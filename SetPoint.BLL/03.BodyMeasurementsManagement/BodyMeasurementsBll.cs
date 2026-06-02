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
        #endregion
    }
}
