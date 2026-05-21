using System.ComponentModel.DataAnnotations.Schema;

namespace SetPoint.DAL._1.Entity
{
    [Table("MUSCLE_GROUP")]
    public class MuscleGroup : BaseEntity
    {
        [Column("NAME", Order = 6)]
        public required string Name { get; set; }

        [Column("DESCRIPTION", Order = 7)]
        public string? Description { get; set; }
    }
}
