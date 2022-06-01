using System.Text.Json.Serialization;

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
        [JsonPropertyName("href")]
        public string? Href { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="string"/> value that identifies a relationship for a resource. This attribute is itself an object and has “rel” “href” attributes.
        /// </summary>
        [JsonPropertyName("rel")]
        public string? Rel { get; set; }
    }
}
