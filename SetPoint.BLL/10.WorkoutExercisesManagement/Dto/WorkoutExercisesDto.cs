using System.Text.Json.Serialization;

namespace SetPoint.BLL._10.WorkoutExercisesManagement.Dto
{
    public class WorkoutExercisesDto
    {
        [JsonPropertyName("ID")]
        public Guid Id { get; set; }

        [JsonPropertyName("CREATED_AT")]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("UPDATED_AT")]
        public DateTime? UpdatedAt { get; set; }

        [JsonPropertyName("DELETED_AT")]
        public DateTime? DeletedAt { get; set; }

        [JsonPropertyName("SESSION_ID")]
        public Guid SessionId { get; set; }

        [JsonPropertyName("EXERCISE_ID")]
        public Guid ExerciseId { get; set; }

        [JsonPropertyName("SORT_ORDER")]
        public int Order { get; set; }

        [JsonPropertyName("REST_SECOND")]
        public int RestSecond { get; set; } = 0;
    }
}
