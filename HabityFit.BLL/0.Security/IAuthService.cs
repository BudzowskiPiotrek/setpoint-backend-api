using HabityFit.BLL._02.UsersManagement.Dto;

namespace HabityFit.BLL._1.Security
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> AuthenticateAsync(string email, string password);
    }
}
