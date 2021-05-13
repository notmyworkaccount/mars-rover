using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NASAClient.Models
{
    public class EarthDateResponse
    {
        [JsonPropertyName("photos")]
        public List<Photo> Photos { get; set; }
    }


}
