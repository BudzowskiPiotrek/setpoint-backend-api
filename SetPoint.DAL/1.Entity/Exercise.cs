using System.ComponentModel.DataAnnotations.Schema;

namespace SetPoint.DAL._1.Entity
{
    [Table("EXERCISE")]
    public class Exercise : BaseEntity
    {
        [Column("NAME", Order = 6)]
        public required string Name { get; set; }

        [Column("DESCRIPTION", Order = 7)]
        public string? Description { get; set; }

        [Column("IMAGE_URL", Order = 8)]
        public string? ImageUrl { get; set; }

        [Column("EQUIPMENT_TYPE", Order = 9)]
        public string? EquipmentType { get; set; }
    }
}
