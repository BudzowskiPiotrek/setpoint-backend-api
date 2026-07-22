using System.ComponentModel.DataAnnotations.Schema;

namespace SetPoint.DAL._1.Entity
{
    /// <summary>
    /// Persistence entity representing a muscle group used to categorize exercises.
    /// </summary>
    [Table("MUSCLE_GROUP")]
    public class MuscleGroup : BaseEntity
    {
        /// <summary>
        /// Gets or sets the display name of the muscle group.
        /// </summary>
        [Column("NAME", Order = 6)]
        public required string Name { get; set; }

        /// <summary>
        /// Gets or sets an optional description of the muscle group.
        /// </summary>
        [Column("DESCRIPTION", Order = 7)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the minimum recommended weekly sets for this muscle group.
        /// </summary>
        [Column("MIN_SETS", Order = 8)]
        public int? MinSets { get; set; }

        /// <summary>
        /// Gets or sets the maximum recommended weekly sets for this muscle group.
        /// </summary>
        [Column("MAX_SETS", Order = 9)]
        public int? MaxSets { get; set; }
    }
}
