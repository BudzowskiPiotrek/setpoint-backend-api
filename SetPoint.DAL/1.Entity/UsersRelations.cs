using System.ComponentModel.DataAnnotations.Schema;

namespace SetPoint.DAL._1.Entity
{
    public enum RelationStatus
    {
        Pending = 1,
        Accepted = 2,
        Rejected = 3
    }
    [Table("USERS_RELATIONS")]
    public class UsersRelations : BaseEntity
    {
        [Column("USER_ID", Order = 5)]
        public Guid UserId { get; set; }

        [Column("FRIEND_ID", Order = 6)]
        public Guid FriendId { get; set; }

        [Column("STATUS", Order = 7)]
        public RelationStatus Status { get; set; } = RelationStatus.Pending;

        [ForeignKey("UserId")]
        public virtual Users User { get; set; } = null!;

        [ForeignKey("FriendId")]
        public virtual Users Friend { get; set; } = null!;
    }
}