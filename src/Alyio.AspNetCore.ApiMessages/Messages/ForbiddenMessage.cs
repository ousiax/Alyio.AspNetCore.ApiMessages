using System;

namespace Alyio.AspNetCore.ApiMessages
{
    /// <summary>
    /// Equivalent to HTTP status 403. <see cref="ForbiddenMessage"/> indicates that the server refuses to fulfill the request.
    /// </summary>
    public sealed class ForbiddenMessage : Exception, IApiMessage
    {
        /// <summary>
        /// Initialize a new instance of <see cref="ForbiddenMessage"/> class.
        /// </summary>
        public ForbiddenMessage() : this(XMessage.Forbidden)
        {
        }

        /// <summary>
        /// Initialize a new instance of <see cref="ForbiddenMessage"/> class.
        /// </summary>
        public ForbiddenMessage(string message) : base(message)
        {
            this.ApiMessage = new ApiMessage { Message = message };
        }

        /// <inheritdoc />
        public ApiMessage ApiMessage { get; }

        /// <summary>
        /// 403
        /// </summary>
        public int StatusCode { get; } = 403;
    }
}
