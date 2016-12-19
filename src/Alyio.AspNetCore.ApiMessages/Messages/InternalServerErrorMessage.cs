using System;

namespace Alyio.AspNetCore.ApiMessages
{
    /// <summary>
    /// Equivalent to HTTP status 500. <see cref="InternalServerErrorMessage"/> indicates that a generic error has occurred on the server.
    /// </summary>
    public class InternalServerErrorMessage : Exception, IApiMessage
    {
        public InternalServerErrorMessage() : this(XMessage.InternalServerError)
        {
        }

        public InternalServerErrorMessage(string message) : base(message)
        {
            this.ApiMessage = new ApiMessage { Message = message };
        }

        public ApiMessage ApiMessage { get; }

        public int StatusCode { get; } = 500;
    }
}
