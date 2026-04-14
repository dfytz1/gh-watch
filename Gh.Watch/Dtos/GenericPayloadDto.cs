using Newtonsoft.Json;

namespace Gh.Watch.Dtos
{
    public class GenericPayloadDto
    {
        [JsonProperty("data")]
        public string Data { get; set; }
    }
}