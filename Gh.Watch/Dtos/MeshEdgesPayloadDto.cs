using Gh.Watch.Constants;
using Newtonsoft.Json;

namespace Gh.Watch.Dtos
{
    // Flat float array of line-segment pairs built from mesh topology edges.
    // Same buffer format as BrepEdgesPayloadDto — ready for Three.js LineSegments.
    public class MeshEdgesPayloadDto
    {
        [JsonProperty("buffer")]
        public float[] Buffer { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; } = GeometryType.MeshEdges;
    }
}
