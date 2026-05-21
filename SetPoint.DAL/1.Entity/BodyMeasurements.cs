using System.ComponentModel.DataAnnotations.Schema;

namespace SetPoint.DAL._1.Entity
{
    [Table("BODY_MEASUREMENTS")]
    public class BodyMeasurements : BaseEntity
    {
        [Column("ID_USER", Order = 6)]
        public Guid IdUser { get; set; }

        [Column("DATE", Order = 7)]
        public DateTime Date { get; set; }

        [Column("WEIGHT", Order = 8)]
        public double Weight { get; set; }

        [Column("MUSCLE_MASS", Order = 9)]
        public double? MuscleMass { get; set; }

        [Column("FAT_MASS", Order = 10)]
        public double? FatMass { get; set; }

        [Column("BODY_WATER", Order = 11)]
        public double? BodyWater { get; set; }

        [ForeignKey("IdUser")]
        public required Users User { get; set; }
    }
}
