using System.Text.Json.Serialization;

namespace SetPoint.BLL._11.ExerciseSetsManagement.Dto
{
    public class ExerciseSetsDto
    {
        [JsonPropertyName("ID")]
        public Guid Id { get; set; }

        [JsonPropertyName("CREATED_AT")]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("UPDATED_AT")]
        public DateTime? UpdatedAt { get; set; }

        [JsonPropertyName("DELETED_AT")]
        public DateTime? DeletedAt { get; set; }

        [JsonPropertyName("WORKOUT_EXERCISE_ID")]
        public Guid WorkoutExerciseId { get; set; }

        [JsonPropertyName("SET_NUMBER")]
        public int SetNumber { get; set; }

        [JsonPropertyName("REPS")]
        public int Reps { get; set; }

        [JsonPropertyName("WEIGHT")]
        public double? Weight { get; set; }

        [JsonPropertyName("RPE")]
        public double? Rpe { get; set; }
    }
}