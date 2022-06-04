using System;
using System.Net;

namespace Alyio.AspNetCore.ApiMessages;

/// <summary>
/// Equivalent to HTTP status 500. <see cref="InternalServerErrorMessage"/> indicates that a generic error has occurred on the server.
/// </summary>
public class InternalServerErrorMessage : Exception, IApiMessage
{
    /// <summary>
    /// Initialize a new instance of <see cref="InternalServerErrorMessage"/> class.
    /// </summary>
    public InternalServerErrorMessage() : this(XMessage.InternalServerError)
    {
    }

    /// <summary>
    /// Initialize a new instance of <see cref="InternalServerErrorMessage"/> class.
    /// </summary>
    public InternalServerErrorMessage(string message) : base(message)
    {
        this.ProblemDetails = new ProblemDetails
        {
            Status = (int)HttpStatusCode.InternalServerError,
            Title = message,
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
        };
    }

    /// <inheritdoc />
    public ProblemDetails ProblemDetails { get; }
}
