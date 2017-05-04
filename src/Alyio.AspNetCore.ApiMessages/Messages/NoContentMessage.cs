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
        /// <summary>
        /// Initialize a new instance of <see cref="NoContentMessage"/> class.
        /// </summary>
        public NoContentMessage() : this(XMessage.NoContent)
        {
        }

        /// <summary>
        /// Initialize a new instance of <see cref="NoContentMessage"/> class.
        /// </summary>
        public NoContentMessage(string message) : base(message)
        {
            this.ApiMessage = new ApiMessage { Message = message };
        }

        /// <inheritdoc />
        public ApiMessage ApiMessage { get; }

        /// <summary>
        /// 204
        /// </summary>
        public int StatusCode { get; } = 204;
    }
}
