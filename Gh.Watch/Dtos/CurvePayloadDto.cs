

using Gh.Watch.Constants;
using Newtonsoft.Json;

namespace Gh.Watch.Dtos
{
    public class CurvePayloadDto
    {
        [JsonProperty("buffer")]
        public float[] Buffer { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; } = GeometryType.Curve;
    }
}
