using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SetPoint.DAL._1.Entity
{
    public class BaseEntity
    {
        [Key]
        [Column("ID", Order = 1)]
        public Guid Id { get; set; }

        [Column("CREATED_AT", Order = 2)]
        public DateTime CreatedAt { get; set; }

        [Column("UPDATED_AT", Order = 3)]
        public DateTime? UpdatedAt { get; set; }

        [Column("DELETED_AT", Order = 4)]
        public DateTime? DeletedAt { get; set; }
    }
}
