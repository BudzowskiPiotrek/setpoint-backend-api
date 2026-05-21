using System.ComponentModel.DataAnnotations.Schema;

namespace SetPoint.DAL._1.Entity
{
    [Table("ROUTINES")]
    public class Routines : BaseEntity
    {
        [Column("USER_ID", Order = 6)]
        public Guid UserId { get; set; }

        [Column("NAME", Order = 7)]
        public required string Name { get; set; }

        [Column("DESCRIPTION", Order = 8)]
        public string? Description { get; set; }

        [ForeignKey("UserId")]
        public required Users User { get; set; }
    }
}
