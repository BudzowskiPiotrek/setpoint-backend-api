using System.ComponentModel.DataAnnotations.Schema;

namespace SetPoint.DAL._1.Entity
{
    /// <summary>
    /// Represents the biological sex of the user for profile customization and analytics.
    /// </summary>
    /// 
    public enum Sex
    {
        Male = 1,
        Female = 2
    }

    /// <summary>
    /// Persistence entity representing the application users.
    /// </summary>
    [Table("USERS")]
    public class Users : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique email address of the user, used for authentication.
        /// </summary>
        [Column("EMAIL", Order = 6)]
        public required string Email { get; set; }

        /// <summary>
        /// Gets or sets the secure hashed password of the user.
        /// </summary>
        [Column("PASSWORD_HASH", Order = 7)]
        public required string PasswordHash { get; set; }

        /// <summary>
        /// Gets or sets the user's full name.
        /// </summary>
        [Column("NAME", Order = 8)]
        public required string FullName { get; set; }

        /// <summary>
        /// Gets or sets the user's biological sex.
        /// </summary>
        [Column("SEX", Order = 9)]
        public Sex? Sex { get; set; }

        /// <summary>
        /// Gets or sets the user's date of birth.
        /// </summary>
        [Column("BIRTH_DATE", Order = 10)]
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// Gets or sets the user's height in centimeters.
        /// </summary>
        [Column("HEIGHT", Order = 11)]
        public double? Height { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the user accepted the latest Terms and Conditions.
        /// </summary>
        [Column("TERMS_ACCEPTED_AT", Order = 12)]
        public DateTime? TermsAcceptedAt { get; set; }
    }
}
