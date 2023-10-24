using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace Alyio.AspNetCore.ApiMessages;

/// <summary>
/// Equivalent to HTTP status 401. <see cref="UnauthorizedMessage"/> indicates
/// that the requested resource requires authentication. The WWW-Authenticate header
/// contains the details of how to perform the authentication.
/// </summary>
public sealed class UnauthorizedMessage : Exception, IApiMessage
{
    /// <summary>
    /// Initialize a new instance of <see cref="UnauthorizedMessage"/> class.
    /// </summary>
    public UnauthorizedMessage()
    {
        this.ProblemDetails = new ProblemDetails
        {
            Status = (int)HttpStatusCode.Unauthorized,
            Title = XMessage.Unauthorized,
            Type = "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1",
        };
    }

    /// <summary>
    /// Initialize a new instance of <see cref="UnauthorizedMessage"/> class.
    /// </summary>
    public UnauthorizedMessage(string detail) : this()
    {
        this.ProblemDetails.Detail = detail;
    }

    /// <inheritdoc />
    public ProblemDetails ProblemDetails { get; }
}

