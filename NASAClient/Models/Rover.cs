using System.Text.Json.Serialization;

namespace NASAClient.Models
{
    public class Rover
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("landing_date")]
        public string LandingDate { get; set; }

        [JsonPropertyName("launch_date")]
        public string LaunchDate { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }


}
