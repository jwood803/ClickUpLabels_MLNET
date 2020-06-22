using System.Text.Json.Serialization;

namespace ClickUpData.Models
{
    public class Folder
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}
