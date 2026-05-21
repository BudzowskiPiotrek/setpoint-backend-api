using System.ComponentModel.DataAnnotations.Schema;

namespace SetPoint.DAL._1.Entity
{
    public enum RequestStatus
    {
        Pending = 1,
        Accepted = 2,
        Rejected = 3
    }
    [Table("ROUTINE_REQUESTS")]
    public class RoutineRequests : BaseEntity
    {
        [Column("SENDER_ID", Order = 5)]
        public Guid SenderId { get; set; }
        [Column("RECEIVER_ID", Order = 6)]
        public Guid ReceiverId { get; set; }
        [Column("ROUTINE_ID", Order = 7)]
        public Guid RoutineId { get; set; }
        [Column("STATUS", Order = 8)]
        public RequestStatus Status { get; set; } = RequestStatus.Pending;


        [ForeignKey("SenderId")]
        public required Users Sender { get; set; } = null!;
        [ForeignKey("ReceiverId")]
        public required Users Receiver { get; set; } = null!;
        [ForeignKey("RoutineId")]
        public required Routines Routine { get; set; } = null!;
    }
}
