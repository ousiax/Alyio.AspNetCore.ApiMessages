using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Alyio.AspNetCore.ApiMessages;

/// <summary>
/// Equivalent to HTTP status 403. <see cref="ForbiddenMessage"/> indicates that the server refuses to fulfill the request.
/// </summary>
public sealed class ForbiddenMessage : Exception, IApiMessage
{
    /// <summary>
    /// Initialize a new instance of <see cref="ForbiddenMessage"/> class.
    /// </summary>
    public ForbiddenMessage()
    {
        this.ProblemDetails = new ProblemDetails
        {
            Title = XMessage.Forbidden,
            Status = StatusCodes.Status403Forbidden,
            Type = StatusCodeTypes.Status403Forbidden,
        };
    }

    /// <summary>
    /// Initialize a new instance of <see cref="ForbiddenMessage"/> class.
    /// </summary>
    public ForbiddenMessage(string detail) : this()
    {
        this.ProblemDetails.Detail = detail;
    }

    /// <inheritdoc />
    public ProblemDetails ProblemDetails { get; }
}
