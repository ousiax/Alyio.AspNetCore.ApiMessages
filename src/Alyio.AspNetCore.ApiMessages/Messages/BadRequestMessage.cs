using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Alyio.AspNetCore.ApiMessages
{
    /// <summary>
    /// Equivalent to HTTP status 400. <see cref="BadRequestMessage"/> indicates
    /// that the request could not be understood by the server. <see cref="BadRequestMessage"/>
    /// is sent when no other error is applicable, or if the exact error is unknown or
    /// does not have its own error code.
    /// </summary>
    public sealed class BadRequestMessage : Exception, IApiMessage
    {
        public BadRequestMessage(string message) : base(message)
        {
            this.ApiMessage = new ApiMessage { Message = message };
        }

        public BadRequestMessage(string message, params string[] errors) : base(message)
        {
            this.ApiMessage = new ApiMessage { Message = message, Errors = errors };
        }

        public BadRequestMessage(ModelStateDictionary modelState) : this(XMessage.ValidationFailed, modelState)
        {
        }

        public BadRequestMessage(string message, ModelStateDictionary modelState) : base(message)
        {
            this.ApiMessage = new ApiMessage { Message = message };
            if (modelState.ErrorCount > 0 && this.ApiMessage.Errors == null)
            {
                this.ApiMessage.Errors = new List<string>();
                foreach (var stateEntry in modelState.Values)
                {
                    foreach (var error in stateEntry.Errors)
                    {
                        this.ApiMessage.Errors.Add(error.ErrorMessage);
                    }
                }
            }
        }

        public ApiMessage ApiMessage { get; }

        public int StatusCode { get; } = 400;
    }
}
