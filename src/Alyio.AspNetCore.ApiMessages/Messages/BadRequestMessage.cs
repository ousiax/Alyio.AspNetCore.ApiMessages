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
    public BadRequestMessage() : this(XMessage.ValidationFailed)
    {
    }

    /// <summary>
    /// Initialize a new instance of <see cref="BadRequestMessage"/> class.
    /// </summary>
    public BadRequestMessage(string message) : base(message)
    {
        this.ProblemDetails = new ProblemDetails
        {
            Title = message,
            Status = (int)HttpStatusCode.BadRequest,
        };
    }

    /// <summary>
    /// Initialize a new instance of <see cref="BadRequestMessage"/> class.
    /// </summary>
    public BadRequestMessage(string message, params string[] errors) : this(message)
    {
        this.ProblemDetails.Extensions["errors"] = errors;
    }

    /// <summary>
    /// Initialize a new instance of <see cref="BadRequestMessage"/> class.
    /// </summary>
    public BadRequestMessage(ModelStateDictionary modelState) : this(XMessage.ValidationFailed, modelState)
    {
    }

    /// <summary>
    /// Initialize a new instance of <see cref="BadRequestMessage"/> class.
    /// </summary>
    public BadRequestMessage(string message, ModelStateDictionary modelState) : this(message)
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
