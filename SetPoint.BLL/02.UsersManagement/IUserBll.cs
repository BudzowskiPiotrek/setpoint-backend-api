using SetPoint.BLL._02.UsersManagement.Dto;

namespace SetPoint.BLL._02.UsersManagement
{
    public interface IUserBll
    {
        //################################################# Authentication
        Task<LoginResponseDto?> Login(LoginRequestDto loginRequest);
        Task<bool> Logout(Guid userId);

        //################################################# User Management
        Task<bool> CreateUser(UserDto user);
        Task<bool> UpdateUser(UserDto user);
        Task<bool> DeleteUser(Guid userId);

        //################################################# User Retrieval
        Task<UserReadDto?> GetUserById(Guid userId);
        Task<UserReadDto?> GetUserByEmail(string email);
        Task<IEnumerable<UserReadDto>> GetAllUsers();
        Task<bool> SyncUser(UserReadDto dto);
    }
}
