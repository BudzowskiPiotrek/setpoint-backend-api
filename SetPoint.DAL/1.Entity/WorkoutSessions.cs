
using System.ComponentModel.DataAnnotations.Schema;

namespace SetPoint.DAL._1.Entity
{
    [Table("WORKOUT_SESSIONS")]
    public class WorkoutSessions : BaseEntity
    {
        [Column("USER_ID", Order = 6)]
        public Guid UserId { get; set; }

        [Column("DATE", Order = 7)]
        public DateTime Date { get; set; }

        [Column("DURATION", Order = 8)]
        public int DurationMinutes { get; set; }

        [Column("NOTES", Order = 9)]
        public string? Notes { get; set; }

        [ForeignKey("UserId")]
        public required Users User { get; set; }
    }
}
