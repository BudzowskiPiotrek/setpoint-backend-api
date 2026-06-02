using System.ComponentModel.DataAnnotations.Schema;

public enum FeedEventType
{
    START_SESSION = 1,
    NEW_RECORD = 2,
    CHALLENGE_WIN = 3,
    GENERAL = 4
}


namespace SetPoint.DAL._1.Entity
{
    [Table("FEED_EVENTS")]
    public class FeedEvent : BaseEntity
    {
        [Column("USER_ID", Order = 5)]
        public Guid? UserId { get; set; }

        [Column("EVENT_TYPE", Order = 6)]
        public FeedEventType EventType { get; set; }

        [Column("EXERCISE_ID", Order = 7)]
        public Guid? ExerciseId { get; set; } = null;

        [Column("METRIC_VALUE", Order = 8)]
        public double? MetricValue { get; set; } = null;

        [Column("CUSTOM_TEXT", Order = 9)]
        public string? CustomText { get; set; } = null;

        /// <summary>
        /// FOREIGN KEY to USERS table
        /// </summary>
        [ForeignKey("UserId")]
        public Users? User { get; set; }

        [ForeignKey("ExerciseId")]
        public Exercise? Exercise { get; set; }
    }
}
