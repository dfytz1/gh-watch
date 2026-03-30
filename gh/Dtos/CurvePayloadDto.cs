

using gh.Constants;
using Newtonsoft.Json;

namespace gh.Dtos
{
    public class CurvePayloadDto
    {
        [JsonProperty("buffer")]
        public float[] Buffer { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; } = GeometryType.Curve;
    }
}
