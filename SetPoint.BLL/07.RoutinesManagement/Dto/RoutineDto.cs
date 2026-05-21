using System.Text.Json.Serialization;

namespace SetPoint.BLL._07.RoutinesManagement.Dto
{
    public class RoutineDto
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

        [JsonPropertyName("NAME")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("DESCRIPTION")]
        public string? Description { get; set; }
    }
}
