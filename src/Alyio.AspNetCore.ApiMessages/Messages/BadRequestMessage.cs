using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace Alyio.AspNetCore.ApiMessages;

/// <summary>
/// Equivalent to HTTP status 400. <see cref="BadRequestMessage"/> indicates
/// that the request could not be understood by the server. <see cref="BadRequestMessage"/>
/// is sent when no other error is applicable, or if the exact error is unknown or
/// does not have its own error code.
/// </summary>
public sealed class BadRequestMessage : Exception, IApiMessage
{
    /// <summary>
    /// Initialize a new instance of <see cref="BadRequestMessage"/> class with default 'ValidationFailed' message.
    /// </summary>
    public BadRequestMessage()
    {
        this.ProblemDetails = new ProblemDetails
        {
            Title = XMessage.ValidationFailed,
            Status = StatusCodes.Status400BadRequest,
            Type = StatusCodeTypes.Status400BadRequest,
        };
    }

    /// <summary>
    /// Initialize a new instance of <see cref="BadRequestMessage"/> class.
    /// </summary>
    public BadRequestMessage(string detail) : this()
    {
        this.ProblemDetails.Detail = detail;
    }

    /// <summary>
    /// Initialize a new instance of <see cref="BadRequestMessage"/> class.
    /// </summary>
    public BadRequestMessage(string detail, params string[] errors) : this(detail)
    {
        this.ProblemDetails.Extensions["errors"] = errors;
    }

    /// <summary>
    /// Initialize a new instance of <see cref="BadRequestMessage"/> class.
    /// </summary>
    public BadRequestMessage(ModelStateDictionary modelState) : this(null!, modelState)
    {
    }

    /// <summary>
    /// Initialize a new instance of <see cref="BadRequestMessage"/> class.
    /// </summary>
    public BadRequestMessage(string detail, ModelStateDictionary modelState) : this(detail)
    {
        if (modelState.ErrorCount > 0)
        {
            this.ProblemDetails = new ValidationProblemDetails(modelState)
            {
                Title = XMessage.ValidationFailed,
                Status = StatusCodes.Status400BadRequest,
                Type = StatusCodeTypes.Status400BadRequest,
            };

            if (detail != null)
            {
                this.ProblemDetails.Detail = detail;
            }
        }
    }

    /// <inheritdoc />
    public ProblemDetails ProblemDetails { get; private set; }
}
