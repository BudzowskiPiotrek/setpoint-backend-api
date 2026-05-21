namespace SetPoint.BLL._03.BodyMeasurementsManagement.Dto
{
    public interface IBodyMeasurementsBll
    {
        Task<bool> SyncBody(BodyMeasurementsDto dto);
        Task<bool> CreateBody(BodyMeasurementsDto bodyDto);
        Task<bool> UpdateBody(BodyMeasurementsDto bodyDto);
        Task<BodyMeasurementsDto?> GetBodyById(Guid id);
        Task<IEnumerable<BodyMeasurementsDto>> GetAllBodyByUserId(Guid userId);
        Task<bool> DeleteBody(Guid id);
    }
}
