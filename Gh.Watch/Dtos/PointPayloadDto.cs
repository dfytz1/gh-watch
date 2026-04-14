using Gh.Watch.Constants;
using Newtonsoft.Json;


namespace Gh.Watch.Dtos
{
    public class PointPayloadDto
    {
        [JsonProperty("x")]
        public float X { get; set; }

        [JsonProperty("y")]
        public float Y { get; set; }

        [JsonProperty("z")]
        public float Z { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; } = GeometryType.Point;
    }
}