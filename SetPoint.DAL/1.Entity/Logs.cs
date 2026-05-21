using System.ComponentModel.DataAnnotations.Schema;

namespace SetPoint.DAL._1.Entity
{
    [Table("LOGS")]
    public class Logs : BaseEntity
    {
        [Column("USER_ID", Order = 6)]
        public Guid? UserId { get; set; }

        [Column("TYPE", Order = 7)]
        public string? Type { get; set; }

        [ForeignKey("UserId")]
        public Users? User { get; set; }
    }
}
