using SetPoint.DAL._1.Entity;
using System.Text.Json.Serialization;

namespace SetPoint.BLL._07.RoutineRequestManagement.Dto
{
    public class RoutineRequestDto
    {
        [JsonPropertyName("ID")]
        public Guid Id { get; set; }

        [JsonPropertyName("CREATED_AT")]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("UPDATED_AT")]
        public DateTime? UpdatedAt { get; set; }

        [JsonPropertyName("DELETED_AT")]
        public DateTime? DeletedAt { get; set; }

        [JsonPropertyName("SENDER_ID")]
        public Guid SenderId { get; set; }

        [JsonPropertyName("RECEIVER_ID")]
        public Guid ReceiverId { get; set; }

        [JsonPropertyName("ROUTINE_ID")]
        public Guid RoutineId { get; set; }

        [JsonPropertyName("STATUS")]
        public RequestStatus Status { get; set; }
    }
}
