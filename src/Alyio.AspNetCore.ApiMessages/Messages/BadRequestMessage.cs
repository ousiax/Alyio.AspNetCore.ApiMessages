using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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
            Status = (int)HttpStatusCode.BadRequest,
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
            var errors = new List<string>();
            foreach (var key in modelState.Keys)
            {
                var stateEntry = modelState[key];
                foreach (var error in stateEntry!.Errors)
                {
                    errors.Add($"{key}: {error.ErrorMessage}");
                }
            }
            this.ProblemDetails.Extensions["errors"] = errors;
        }
    }

    /// <inheritdoc />
    public ProblemDetails ProblemDetails { get; }
}
