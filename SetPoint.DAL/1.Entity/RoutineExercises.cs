using System.ComponentModel.DataAnnotations.Schema;

namespace SetPoint.DAL._1.Entity
{
    [Table("ROUTINE_Exercises")]
    public class RoutineExercises : BaseEntity
    {
        [Column("ROUTINE_ID", Order = 6)]
        public Guid RoutineId { get; set; }

        [Column("EXERCISE_ID", Order = 7)]
        public Guid ExerciseId { get; set; }

        [Column("SORT_ORDER", Order = 8)]
        public int Order { get; set; }

        [Column("SETS", Order = 9)]
        public int Sets { get; set; }

        [Column("REPS", Order = 10)]
        public int Reps { get; set; }

        [Column("TARGET_WEIGHT", Order = 11)]
        public double? TargetWeight { get; set; }

        [Column("REST_SECOND", Order = 12)]
        public int RestSecond { get; set; }

        [ForeignKey("RoutineId")]
        public required Routines Routine { get; set; }

        [ForeignKey("ExerciseId")]
        public required Exercise Exercise { get; set; }
    }
}
