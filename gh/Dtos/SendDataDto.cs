using Newtonsoft.Json;

namespace gh.Dtos
{
    public class SendDataDto
    {
        [JsonProperty("eventType")]
        public string EventType { get; set; }

        [JsonProperty("payload")]
        public object Payload { get; set; }
    }
}