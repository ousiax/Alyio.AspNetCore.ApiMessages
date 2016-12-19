using Newtonsoft.Json;

namespace Alyio.AspNetCore.ApiMessages
{
    public sealed class Link
    {
        [JsonProperty("href", Required = Required.Always)]
        public string Href { get; set; }

        [JsonProperty("rel", Required = Required.Always)]
        public string Rel { get; set; }
    }
}
