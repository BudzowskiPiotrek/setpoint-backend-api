using HabityFit.BLL._02.UsersManagement.Dto;

namespace HabityFit.BLL._1.Security
{
    public interface ITokenService
    {
        string CreateToken(UserReadDto user);
    }
}
