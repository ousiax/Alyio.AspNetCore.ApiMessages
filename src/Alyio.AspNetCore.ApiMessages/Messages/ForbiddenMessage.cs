using System;

namespace Alyio.AspNetCore.ApiMessages
{
    /// <summary>
    /// Equivalent to HTTP status 403. <see cref="ForbiddenMessage"/> indicates that the server refuses to fulfill the request.
    /// </summary>
    public sealed class ForbiddenMessage : Exception, IApiMessage
    {
        public ForbiddenMessage() : this(XMessage.Forbidden)
        {
        }

        public ForbiddenMessage(string message) : base(message)
        {
            this.ApiMessage = new ApiMessage { Message = message };
        }

        public ApiMessage ApiMessage { get; }

        public int StatusCode { get; } = 403;
    }
}
