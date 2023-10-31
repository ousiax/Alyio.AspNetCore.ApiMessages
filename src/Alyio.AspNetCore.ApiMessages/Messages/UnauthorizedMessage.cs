using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

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
            Title = XMessage.Unauthorized,
            Status = StatusCodes.Status401Unauthorized,
            Type = StatusCodeTypes.Status401Unauthorized,
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

