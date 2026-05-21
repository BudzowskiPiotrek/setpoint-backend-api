using SetPoint.BLL._02.UsersManagement.Dto;

namespace SetPoint.BLL._1.Security
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> AuthenticateAsync(string email, string password);
    }
}
