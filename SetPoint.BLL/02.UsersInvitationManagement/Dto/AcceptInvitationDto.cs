using System.Text.Json.Serialization;

namespace SetPoint.BLL._02.UsersInvitationManagement.Dto
{
    public class AcceptInvitationDto
    {
        [JsonPropertyName("Token")]
        public required Guid Token { get; set; }

        [JsonPropertyName("Email")]
        public required string FullName { get; set; }

        [JsonPropertyName("Password")]
        public required string Password { get; set; }
    }
}
