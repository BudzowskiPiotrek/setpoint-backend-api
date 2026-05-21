using System.Text.Json.Serialization;

namespace SetPoint.BLL._08.RoutineExercisesManagement.Dto
{
    public class RoutineExerciseDto
    {
        [JsonPropertyName("ID")]
        public Guid Id { get; set; }

        [JsonPropertyName("CREATED_AT")]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("UPDATED_AT")]
        public DateTime? UpdatedAt { get; set; }

        [JsonPropertyName("DELETED_AT")]
        public DateTime? DeletedAt { get; set; }

        [JsonPropertyName("ROUTINE_ID")]
        public Guid RoutineId { get; set; }

        [JsonPropertyName("EXERCISE_ID")]
        public Guid ExerciseId { get; set; }

        [JsonPropertyName("SORT_ORDER")]
        public int Order { get; set; }

        [JsonPropertyName("SETS")]
        public int Sets { get; set; }

        [JsonPropertyName("REPS")]
        public int Reps { get; set; }

        [JsonPropertyName("TARGET_WEIGHT")]
        public double? TargetWeight { get; set; }

        [JsonPropertyName("REST_SECOND")]
        public int RestSecond { get; set; }
    }
}