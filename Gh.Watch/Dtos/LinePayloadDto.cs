

using Gh.Watch.Constants;
using Newtonsoft.Json;

namespace Gh.Watch.Dtos
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
