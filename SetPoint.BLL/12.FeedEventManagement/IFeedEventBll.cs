using SetPoint.BLL._12.FeedEventManagement.Dto;

namespace SetPoint.BLL._12.FeedEventManagement
{
    public interface IFeedEventBll
    {
        Task<bool> SyncFeedEvent(FeedEventDto dto);
    }
}
