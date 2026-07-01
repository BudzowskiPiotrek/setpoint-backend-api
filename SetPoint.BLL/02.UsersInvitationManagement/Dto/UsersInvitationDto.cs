using System.Text.Json.Serialization;

namespace SetPoint.BLL._02.UsersInvitationManagement.Dto
{
    public class UsersInvitationDto
    {
        [JsonPropertyName("ID")]
        public required Guid Id { get; set; }

        [JsonPropertyName("EMAIL")]
        public required string Email { get; set; }

        [JsonPropertyName("SENDER_USER_ID")]
        public Guid SenderUserId { get; set; }
    }
}
