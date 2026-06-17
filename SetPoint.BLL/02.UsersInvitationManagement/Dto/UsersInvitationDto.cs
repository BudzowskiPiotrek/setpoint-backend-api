using System.Text.Json.Serialization;

namespace SetPoint.BLL._02.UsersInvitationManagement.Dto
{
    public class UsersInvitationDto
    {
        [JsonPropertyName("Id")]
        public required Guid Id { get; set; }

        [JsonPropertyName("Email")]
        public required string Email { get; set; }

        [JsonPropertyName("SenderUserId")]
        public Guid SenderUserId { get; set; }
    }
}
