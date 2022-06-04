using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Alyio.AspNetCore.ApiMessages;

/// <summary>
/// Represents a api message.
/// </summary>
public class ApiMessage
{
    [MaybeNull]
    private string _message;

    /// <summary>
    /// Gets or sets the api message.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message
    {
        get { return _message!; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("The specified string is null, empty, or consists only of white-space characters.", nameof(value));
            }
            _message = value;
        }
    }

    /// <summary>
    /// Gets or sets a unique identifier to represent this request in trace logs.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Http.HttpContext.TraceIdentifier"/>
    [JsonPropertyName("trace_identifier")]
    public string? TraceIdentifier { get; set; }

    /// <summary>
    /// Gets or sets the model state errors.
    /// </summary>
    [JsonPropertyName("errors")]
    public IList<string>? Errors { get; set; }

    /// <summary>
    /// Gets or sets the exception type.
    /// </summary>
    [JsonPropertyName("exception_type")]
    public string? ExceptionType { get; set; }

    /// <summary>
    /// Gets or sets the exception detail information.
    /// </summary>
    [JsonPropertyName("detail")]
    public string? Detail { get; set; }
}
