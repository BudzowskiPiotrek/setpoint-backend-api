using SetPoint.BLL._02.UsersManagement.Dto;

namespace SetPoint.BLL._1.Security
{
    public interface ITokenService
    {
        string CreateToken(UserReadDto user);
    }
}
