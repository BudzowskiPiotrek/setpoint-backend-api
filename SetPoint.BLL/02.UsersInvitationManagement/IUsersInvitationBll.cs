using SetPoint.BLL._02.UsersInvitationManagement.Dto;
using SetPoint.BLL._02.UsersManagement.Dto;

namespace SetPoint.BLL._02.UsersInvitationManagement
{
    public interface IUsersInvitationBll
    {
        Task<bool> CreateAndSendInvitationAsync(UsersInvitationDto dto);
        Task<LoginResponseDto?> AcceptInvitationAsync(Guid token, string fullName, string password);
    }
}
