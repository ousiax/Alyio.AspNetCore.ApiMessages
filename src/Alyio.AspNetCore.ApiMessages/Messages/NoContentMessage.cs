using System;

namespace Alyio.AspNetCore.ApiMessages
{
    /// <summary>
    /// Equivalent to HTTP status 204. <see cref="NoContentMessage"/> indicates
    /// that the request has been successfully processed and that the response is intentionally
    /// blank.
    /// </summary>
    public class NoContentMessage : Exception, IApiMessage
    {
        public NoContentMessage() : this(XMessage.NoContent)
        {
        }

        public NoContentMessage(string message) : base(message)
        {
            this.ApiMessage = new ApiMessage { Message = message };
        }

        public ApiMessage ApiMessage { get; }

        public int StatusCode { get; } = 204;
    }
}
