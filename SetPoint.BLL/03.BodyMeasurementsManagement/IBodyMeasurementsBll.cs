namespace SetPoint.BLL._03.BodyMeasurementsManagement.Dto
{
    public interface IBodyMeasurementsBll
    {
        Task<bool> SyncBody(BodyMeasurementsDto dto);
    }
}
