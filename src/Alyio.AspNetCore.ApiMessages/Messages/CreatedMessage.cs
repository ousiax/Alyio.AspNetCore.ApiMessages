using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Alyio.AspNetCore.ApiMessages;

/// <summary>
/// Represents a api message that used to produce a Created (201) response.
/// </summary>
public sealed class CreatedMessage
{
    /// <summary>
    /// Gets or sets a <see cref="string"/> value that identities the unique ID of a resource.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the HATEOAS resource links.
    /// </summary>
    [JsonPropertyName("links")]
    public List<Link>? Links { get; set; }
}
