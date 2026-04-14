using Newtonsoft.Json;

namespace Gh.Watch.Dtos
{
    public class SendDataDto
    {
        [JsonProperty("eventType")]
        public string EventType { get; set; }

        [JsonProperty("payload")]
        public object Payload { get; set; }
    }
}