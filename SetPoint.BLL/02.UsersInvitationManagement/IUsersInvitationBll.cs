using SetPoint.BLL._02.UsersInvitationManagement.Dto;

namespace SetPoint.BLL._02.UsersInvitationManagement
{
    public interface IUsersInvitationBll
    {
        Task<bool> CreateAndSendInvitationAsync(UsersInvitationDto dto);
    }
}
