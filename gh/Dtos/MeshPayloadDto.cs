using gh.Constants;
using Newtonsoft.Json;

namespace gh.Dtos
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
