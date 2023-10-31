using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Alyio.AspNetCore.ApiMessages;

/// <summary>
/// Equivalent to HTTP status 500. <see cref="InternalServerErrorMessage"/> indicates that a generic error has occurred on the server.
/// </summary>
public class InternalServerErrorMessage : Exception, IApiMessage
{
    /// <summary>
    /// Initialize a new instance of <see cref="InternalServerErrorMessage"/> class.
    /// </summary>
    public InternalServerErrorMessage()
    {
        this.ProblemDetails = new ProblemDetails
        {
            Title = XMessage.InternalServerError,
            Status = StatusCodes.Status500InternalServerError,
            Type = StatusCodeTypes.Status500InternalServerError,
        };
    }

    /// <summary>
    /// Initialize a new instance of <see cref="InternalServerErrorMessage"/> class.
    /// </summary>
    public InternalServerErrorMessage(string detail) : this()
    {
        this.ProblemDetails.Detail = detail;
    }

    /// <inheritdoc />
    public ProblemDetails ProblemDetails { get; }
}
