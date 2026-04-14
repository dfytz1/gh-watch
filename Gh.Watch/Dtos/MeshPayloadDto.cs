using Gh.Watch.Constants;
using Newtonsoft.Json;

namespace Gh.Watch.Dtos
{
    public class MeshPayloadDto
    {
        [JsonProperty("vertices")]
        public float[] Vertices { get; set; }

        [JsonProperty("faces")]
        public int[] Faces { get; set; }

        [JsonProperty("normals")]
        public float[] Normals { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; } = GeometryType.Mesh;
    }
}
