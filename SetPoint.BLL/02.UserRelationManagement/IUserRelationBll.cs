using SetPoint.BLL._02.UserRelationManagement.Dto;

namespace SetPoint.BLL._02.UserRelationManagement
{
    public interface IUserRelationBll
    {
        Task<bool> SyncUserRelation(UserRelationDto dto);
    }
}
