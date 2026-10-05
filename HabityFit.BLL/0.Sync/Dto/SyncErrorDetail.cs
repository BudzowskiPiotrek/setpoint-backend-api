using System.Text.Json.Serialization;

namespace HabityFit.BLL._0.Sync.Dto
{
    public class SyncErrorDetail
    {
        [JsonPropertyName("itemId")]
        public required List<string> ItemId { get; set; }
        [JsonPropertyName("success")]
        public required List<bool> Success { get; set; }
    }
}
