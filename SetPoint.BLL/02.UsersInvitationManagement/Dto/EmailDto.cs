using System.Text.Json.Serialization;

namespace SetPoint.BLL._02.UsersInvitationManagement.Dto
{
    public class EmailDto
    {
        [JsonPropertyName("TOKEN_AP")]
        public required string Token { get; set; }

        [JsonPropertyName("EMAIL")]
        public required string Email { get; set; }
    }
}
