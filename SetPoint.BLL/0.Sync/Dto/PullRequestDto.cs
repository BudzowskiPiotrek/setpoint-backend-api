using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SetPoint.BLL._0.Sync.Dto
{
    public class PullRequestDto
    {
        [Required]
        [JsonPropertyName("USER_ID")]
        public Guid UserId { get; set; }

        [Required]
        [JsonPropertyName("LAST_SYNC")]
        public DateTime LastSync { get; set; }
    }
}
