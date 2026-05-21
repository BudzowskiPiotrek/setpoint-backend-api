using System.Text.Json.Serialization;

namespace SetPoint.BLL._03.BodyMeasurementsManagement.Dto
{
    public class BodyMeasurementsDto
    {
        [JsonPropertyName("ID")]
        public Guid Id { get; set; }

        [JsonPropertyName("CREATED_AT")]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("UPDATED_AT")]
        public DateTime? UpdatedAt { get; set; }

        [JsonPropertyName("DELETED_AT")]
        public DateTime? DeletedAt { get; set; }

        [JsonPropertyName("ID_USER")]
        public Guid IdUser { get; set; }

        [JsonPropertyName("DATE")]
        public DateTime Date { get; set; }

        [JsonPropertyName("WEIGHT")]
        public double Weight { get; set; }

        [JsonPropertyName("MUSCLE_MASS")]
        public double? MuscleMass { get; set; }

        [JsonPropertyName("FAT_MASS")]
        public double? FatMass { get; set; }

        [JsonPropertyName("BODY_WATER")]
        public double? BodyWater { get; set; }
    }
}
