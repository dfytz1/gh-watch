

using gh.Constants;
using Newtonsoft.Json;

namespace gh.Dtos
{
    public class LinePayloadDto
    {
        [JsonProperty("start")]
        public float[] Start { get; set; }

        [JsonProperty("end")]
        public float[] End { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; } = GeometryType.Line;
    }
}
