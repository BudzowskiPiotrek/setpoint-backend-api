using System.ComponentModel.DataAnnotations.Schema;

namespace SetPoint.DAL._1.Entity
{
    [Table("EXERCISE_MUSCLE_GROUP")]
    public class ExerciseMuscleGroup : BaseEntity
    {
        [Column("EXERCISE_ID", Order = 6)]
        public Guid ExerciseId { get; set; }

        [Column("MUSCLE_GROUP_ID", Order = 7)]
        public Guid MuscleId { get; set; }

        [ForeignKey("ExerciseId")]
        public required Exercise Exercise { get; set; }

        [ForeignKey("MuscleId")]
        public required MuscleGroup MuscleGroup { get; set; }
    }
}
