using System.Text.Json.Serialization;

namespace SetPoint.BLL._02.UsersManagement.Dto
{
    public class LoginResponseDto
    {
        [JsonPropertyName("Token")]
        public string Token { get; set; } = string.Empty;

        [JsonPropertyName("User")]
        public UserReadDto User { get; set; } = null!;

        [JsonPropertyName("Expiration")]
        public DateTime? Expiration { get; set; }
    }
}