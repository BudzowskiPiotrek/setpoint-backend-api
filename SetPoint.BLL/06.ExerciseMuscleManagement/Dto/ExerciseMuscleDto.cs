using System.Text.Json.Serialization;

namespace SetPoint.BLL._06.ExerciseMuscleManagement.Dto
{
    public class ExerciseMuscleDto
    {
        [JsonPropertyName("ID")]
        public Guid Id { get; set; }

        [JsonPropertyName("CREATED_AT")]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("UPDATED_AT")]
        public DateTime? UpdatedAt { get; set; }

        [JsonPropertyName("DELETED_AT")]
        public DateTime? DeletedAt { get; set; }

        [JsonPropertyName("EXERCISE_ID")]
        public Guid ExerciseId { get; set; }

        [JsonPropertyName("MUSCLE_GROUP_ID")]
        public Guid MuscleId { get; set; }
    }
}
