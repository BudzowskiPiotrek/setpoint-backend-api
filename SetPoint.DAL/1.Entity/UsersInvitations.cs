using System.ComponentModel.DataAnnotations.Schema;

namespace SetPoint.DAL._1.Entity
{
    /// <summary>
    /// Represents the various states of a user invitation.
    /// </summary>
    public enum InvitationStatus
    {
        Pending = 1,
        Accepted = 2,
        Expired = 3
    }

    /// <summary>
    /// Persistence entity representing invitations sent to prospective users in the system.
    /// </summary>
    [Table("USER_INVITATIONS")]
    public class UsersInvitations : BaseEntity
    {
        /// <summary>
        /// Gets or sets the target email address where the invitation is sent.
        /// </summary>
        [Column("EMAIL", Order = 5)]
        public required string Email { get; set; }

        /// <summary>
        /// Gets or sets the unique security token used to validate the invitation link.
        /// </summary>
        [Column("TOKEN", Order = 6)]
        public Guid Token { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who generated and sent the invitation.
        /// </summary>
        [Column("SENDER_USER_ID", Order = 7)]
        public Guid? SenderUserId { get; set; }

        /// <summary>
        /// Gets or sets the expiration date and time after which the invitation is no longer valid.
        /// </summary>
        [Column("EXPIRES_AT", Order = 8)]
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Gets or sets the current status of the invitation. Defaults = Pending.
        /// </summary>
        [Column("STATUS", Order = 9)]
        public InvitationStatus Status { get; set; } = InvitationStatus.Pending;

        /// <summary>
        /// Gets or sets a value indicating whether the invitation email has been dispatched by the mail service.
        /// </summary>
        [Column("SENDED", Order = 10)]
        public bool Sended { get; set; }

        /// <summary>
        /// FOREIGN KEY
        /// </summary>
        [ForeignKey("SenderUserId")]
        public Users? SenderUser { get; set; }
    }
}
