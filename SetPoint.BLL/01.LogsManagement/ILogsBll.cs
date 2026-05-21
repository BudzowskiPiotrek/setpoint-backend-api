namespace SetPoint.BLL._01.LogsManagement
{
    public interface ILogsBll
    {
        Task<bool> CreateLogAsync(Guid userId, string type);
    }
}
