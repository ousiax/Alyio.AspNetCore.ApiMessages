using System.Collections.Generic;
using Newtonsoft.Json;

namespace Alyio.AspNetCore.ApiMessages
{
    public sealed class CreatedMessage
    {
        [JsonProperty("id", Required = Required.Always)]
        public string Id { get; set; }

        [JsonProperty("links", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public List<Link> Links { get; set; }
    }
}
