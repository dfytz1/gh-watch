using Gh.Watch.Constants;
using Newtonsoft.Json;

namespace Gh.Watch.Dtos
{
    // Flat float array of line-segment pairs ready for Three.js LineSegments.
    // Format: x0_start,y0_start,z0_start, x0_end,y0_end,z0_end,  <- segment 0
    //         x1_start,y1_start,z1_start, x1_end,y1_end,z1_end,  <- segment 1 ...
    public class BrepEdgesPayloadDto
    {
        [JsonProperty("buffer")]
        public float[] Buffer { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; } = GeometryType.BrepEdges;
    }
}
