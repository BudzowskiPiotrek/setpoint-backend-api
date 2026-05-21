using SetPoint.BLL._02.UsersManagement.Dto;

namespace SetPoint.BLL._02.UsersManagement
{
    public class UserDto : UserReadDto
    {
        public string Password { get; set; } = string.Empty;
    }
}
