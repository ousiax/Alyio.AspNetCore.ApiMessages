using Newtonsoft.Json;

namespace Alyio.AspNetCore.ApiMessages
{
    /// <summary>
    /// Represents a HATEOAS resource link.
    /// </summary>
    public sealed class Link
    {
        /// <summary>
        /// Gets or sets a <see cref="string"/> value that identifies the URL of the current resource.
        /// </summary>
        [JsonProperty("href", Required = Required.Always)]
        public string Href { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="string"/> value that identifies a relationship for a resource. This attribute is itself an object and has “rel” “href” attributes.
        /// </summary>
        [JsonProperty("rel", Required = Required.Always)]
        public string Rel { get; set; }
    }
}
