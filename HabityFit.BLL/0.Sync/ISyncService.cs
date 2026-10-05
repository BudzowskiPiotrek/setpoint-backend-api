using HabityFit.BLL._0.Sync.Dto;

namespace HabityFit.BLL._0.Sync
{
    public interface ISyncService
    {
        Task<SyncPayloadDto?> ProcessPull(PullRequestDto request, Guid userId);
        Task<SyncErrorDetail> ProcessPush(SyncPayloadDto payload, Guid userId);

        //Task<bool> ProcessPush(SyncPayloadDto payload, Guid userId);
    }
}
