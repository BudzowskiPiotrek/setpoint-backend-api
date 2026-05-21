using System.ComponentModel.DataAnnotations.Schema;
public enum Sex
{
    Male = 1,
    Female = 2
}

namespace SetPoint.DAL._1.Entity
{
    [Table("USERS")]
    public class Users : BaseEntity
    {
        [Column("EMAIL", Order = 6)]
        public required string Email { get; set; }

        [Column("PASSWORD_HASH", Order = 7)]
        public required string PasswordHash { get; set; }

        [Column("NAME", Order = 8)]
        public required string FullName { get; set; }

        [Column("SEX", Order = 9)]
        public Sex? Sex { get; set; }

        [Column("BIRTH_DATE", Order = 10)]
        public DateTime? BirthDate { get; set; }

        [Column("HEIGHT", Order = 11)]
        public double? Height { get; set; }
    }
}
