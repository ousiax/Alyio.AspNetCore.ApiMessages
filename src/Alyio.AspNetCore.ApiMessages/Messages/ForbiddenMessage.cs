using System;
using System.Net;

namespace Alyio.AspNetCore.ApiMessages;

/// <summary>
/// Equivalent to HTTP status 403. <see cref="ForbiddenMessage"/> indicates that the server refuses to fulfill the request.
/// </summary>
public sealed class ForbiddenMessage : Exception, IApiMessage
{
    /// <summary>
    /// Initialize a new instance of <see cref="ForbiddenMessage"/> class.
    /// </summary>
    public ForbiddenMessage() : this(XMessage.Forbidden)
    {
    }

    /// <summary>
    /// Initialize a new instance of <see cref="ForbiddenMessage"/> class.
    /// </summary>
    public ForbiddenMessage(string message) : base(message)
    {
        this.ProblemDetails = new ProblemDetails
        {
            Status = (int)HttpStatusCode.Forbidden,
            Title = message,
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3",
        };
    }

    /// <inheritdoc />
    public ProblemDetails ProblemDetails { get; }
}
