using System.Text.Json.Serialization;

namespace HabityFit.BLL._02.UsersManagement.Dto
{
    public class LoginRequestDto
    {
        [JsonPropertyName("Email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("Password")]
        public string Password { get; set; } = string.Empty;
    }
}
