using System;
using Newtonsoft.Json;

namespace HttpPerfDemo
{
    public class Response
    {
        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("sessionId")]
        public string SessionId { get; set; }
    }
}
