using SetPoint.DAL._1.Entity;
using System.Text.Json.Serialization;

namespace SetPoint.BLL._02.UserRelationManagement.Dto
{
    public class UserRelationDto
    {
        [JsonPropertyName("ID")]
        public Guid Id { get; set; }

        [JsonPropertyName("CREATED_AT")]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("UPDATED_AT")]
        public DateTime? UpdatedAt { get; set; }

        [JsonPropertyName("DELETED_AT")]
        public DateTime? DeletedAt { get; set; }

        [JsonPropertyName("USER_ID")]
        public Guid UserId { get; set; }

        [JsonPropertyName("FRIEND_ID")]
        public Guid FriendId { get; set; }

        [JsonPropertyName("STATUS")]
        public RelationStatus Status { get; set; }
    }
}
