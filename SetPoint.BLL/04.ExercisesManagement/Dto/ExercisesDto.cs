using System.Text.Json.Serialization;

namespace SetPoint.BLL._04.ExercisesManagement.Dto
{
    public class ExercisesDto
    {
        [JsonPropertyName("ID")]
        public Guid Id { get; set; }

        [JsonPropertyName("CREATED_AT")]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("UPDATED_AT")]
        public DateTime? UpdatedAt { get; set; }

        [JsonPropertyName("DELETED_AT")]
        public DateTime? DeletedAt { get; set; }

        [JsonPropertyName("NAME")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("DESCRIPTION")]
        public string? Description { get; set; }

        [JsonPropertyName("IMAGE_URL")]
        public string? ImageUrl { get; set; }

        [JsonPropertyName("EQUIPMENT_TYPE")]
        public string? EquipmentType { get; set; }
    }
}
