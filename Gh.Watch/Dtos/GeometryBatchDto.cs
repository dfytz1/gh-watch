using Newtonsoft.Json;
using System.Collections.Generic;

namespace Gh.Watch.Dtos
{
    // Single payload that carries all geometry for one GH solve in one WebView message.
    // brepPayload — JSON-encoded brep objects rendered directly by Three.js.
    // fileData    — raw .3dm bytes (base64 via JSON.NET); null when the file has no objects.
    public class GeometryBatchDto
    {
        [JsonProperty("brepPayload")]
        public List<object> BrepPayload { get; set; }

        [JsonProperty("fileData")]
        public byte[] FileData { get; set; }
    }
}
