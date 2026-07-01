using System.Text.Json.Serialization;

namespace SetPoint.BLL._02.UsersInvitationManagement.Dto
{
    public class AcceptInvitationDto
    {
        [JsonPropertyName("TOKEN")]
        public required Guid Token { get; set; }

        [JsonPropertyName("FULL_NAME")]
        public required string FullName { get; set; }

        [JsonPropertyName("PASSWORD")]
        public required string Password { get; set; }
    }
}
