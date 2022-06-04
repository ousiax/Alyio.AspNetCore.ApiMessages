using System;
using System.Net;

namespace Alyio.AspNetCore.ApiMessages;

/// <summary>
///  Equivalent to HTTP status 404. <see cref="NotFoundMessage"/> indicates that the requested resource does not exist on the server.
/// </summary>
public class NotFoundMessage : Exception, IApiMessage
{
    /// <summary>
    /// Initialize a new instance of <see cref="NotFoundMessage"/> class.
    /// </summary>
    public NotFoundMessage()
    {
        this.ProblemDetails = new ProblemDetails
        {
            Status = (int)HttpStatusCode.NotFound,
            Title = XMessage.NotFound,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
        };
    }

    /// <summary>
    /// Initialize a new instance of <see cref="NotFoundMessage"/> class.
    /// </summary>
    public NotFoundMessage(string detail) : this()
    {
        this.ProblemDetails.Detail = detail;
    }

    /// <inheritdoc />
    public ProblemDetails ProblemDetails { get; }
}
