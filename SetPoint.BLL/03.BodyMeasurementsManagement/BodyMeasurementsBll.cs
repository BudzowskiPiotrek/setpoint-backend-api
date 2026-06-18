using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SetPoint.BLL._03.BodyMeasurementsManagement.Dto;
using SetPoint.DAL._1.Entity;
using SetPoint.DAL._2.Context;

namespace SetPoint.BLL._03.BodyMeasurementsManagement
{
    public class BodyMeasurementsBll : IBodyMeasurementsBll
    {
        #region Fields
        private readonly IMapper _mapper;
        private readonly SetPointDbContext _context;
        #endregion


        #region Constructors
        public BodyMeasurementsBll(SetPointDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        #endregion


        #region Methods
        public async Task<bool> SyncBody(BodyMeasurementsDto dto)
        {
            var existing = await _context.BodyMeasurements
                .FirstOrDefaultAsync(b => b.Id == dto.Id || (b.IdUser == dto.IdUser && b.Date == dto.Date));

            if (existing == null)
            {
                var entity = _mapper.Map<BodyMeasurements>(dto);
                await _context.BodyMeasurements.AddAsync(entity);
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
                    _context.BodyMeasurements.Update(existing);
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
