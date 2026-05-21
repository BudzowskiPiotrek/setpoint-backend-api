using System.ComponentModel.DataAnnotations.Schema;

namespace SetPoint.DAL._1.Entity
{
    [Table("EXERCISE_SETS")]
    public class ExerciseSets : BaseEntity
    {
        [Column("WORKOUT_EXERCISE_ID", Order = 6)]
        public Guid WorkoutExerciseId { get; set; }

        [Column("SET_NUMBER", Order = 7)]
        public int SetNumber { get; set; }

        [Column("REPS", Order = 8)]
        public int Reps { get; set; }

        [Column("WEIGHT", Order = 9)]
        public double? Weight { get; set; }

        [Column("RPE", Order = 10)]
        public double? Rpe { get; set; }

        [ForeignKey("WorkoutExerciseId")]
        public required WorkoutExercises WorkoutExercise { get; set; }
    }
}
