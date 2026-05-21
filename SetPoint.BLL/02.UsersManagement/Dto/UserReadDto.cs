using System.Text.Json.Serialization;

namespace SetPoint.BLL._02.UsersManagement.Dto
{
    public class UserReadDto
    {
        [JsonPropertyName("ID")]
        public Guid Id { get; set; }

        [JsonPropertyName("CREATED_AT")]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("UPDATED_AT")]
        public DateTime? UpdatedAt { get; set; }

        [JsonPropertyName("DELETED_AT")]
        public DateTime? DeletedAt { get; set; }

        [JsonPropertyName("EMAIL")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("NAME")]
        public string FullName { get; set; } = string.Empty;

        [JsonPropertyName("SEX")]
        public Sex? Sex { get; set; }

        [JsonPropertyName("BIRTH_DATE")]
        public DateTime? BirthDate { get; set; }

        [JsonPropertyName("HEIGHT")]
        public double? Height { get; set; }
    }
}
