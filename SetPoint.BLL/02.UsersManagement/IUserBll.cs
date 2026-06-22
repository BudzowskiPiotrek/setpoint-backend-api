using SetPoint.BLL._02.UsersManagement.Dto;

namespace SetPoint.BLL._02.UsersManagement
{
    public interface IUserBll
    {
        Task<LoginResponseDto?> CreateUserAsync(UserDto user);
        Task<LoginResponseDto?> Login(LoginRequestDto loginRequest);
        Task<UserReadDto?> GetUserById(Guid userId);
        Task<bool> SyncUser(UserReadDto dto);
    }
}
