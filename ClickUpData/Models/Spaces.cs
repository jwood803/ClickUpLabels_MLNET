using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUpData.Models
{
    public class Spaces
    {
        [JsonPropertyName("teams")]
        public List<Space> SpacesList { get; set; }
    }
}
