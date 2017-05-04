using System;

namespace Alyio.AspNetCore.ApiMessages
{
    /// <summary>
    ///  Equivalent to HTTP status 404. <see cref="NotFoundMessage"/> indicates that the requested resource does not exist on the server.
    /// </summary>
    public class NotFoundMessage : Exception, IApiMessage
    {
        /// <summary>
        /// Initialize a new instance of <see cref="NotFoundMessage"/> class.
        /// </summary>
        public NotFoundMessage() : this(XMessage.NotFound)
        {
        }

        /// <summary>
        /// Initialize a new instance of <see cref="NotFoundMessage"/> class.
        /// </summary>
        public NotFoundMessage(string message) : base(message)
        {
            this.ApiMessage = new ApiMessage { Message = message };
        }

        /// <inheritdoc />
        public ApiMessage ApiMessage { get; }

        /// <summary>
        /// 404
        /// </summary>
        public int StatusCode { get; } = 404;
    }
}
