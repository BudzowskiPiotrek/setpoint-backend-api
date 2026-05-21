using System.ComponentModel.DataAnnotations.Schema;

namespace SetPoint.DAL._1.Entity
{
    [Table("WORKOUT_EXERCISES")]
    public class WorkoutExercises : BaseEntity
    {
        [Column("SESSION_ID", Order = 6)]
        public Guid SessionId { get; set; }

        [Column("EXERCISE_ID", Order = 7)]
        public Guid ExerciseId { get; set; }

        [Column("SORT_ORDER", Order = 8)]
        public int Order { get; set; }

        [Column("REST_SECOND", Order = 9)]
        public int RestSecond { get; set; } = 0;

        [ForeignKey("SessionId")]
        public required WorkoutSessions WorkoutSession { get; set; }

        [ForeignKey("ExerciseId")]
        public required Exercise Exercise { get; set; }
    }
}
