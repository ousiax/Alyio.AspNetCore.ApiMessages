using System;

namespace Alyio.AspNetCore.ApiMessages
{
    /// <summary>
    ///  Equivalent to HTTP status 404. <see cref="NotFoundMessage"/> indicates that the requested resource does not exist on the server.
    /// </summary>
    public class NotFoundMessage : Exception, IApiMessage
    {
        public NotFoundMessage() : this(XMessage.NotFound)
        {
        }

        public NotFoundMessage(string message) : base(message)
        {
            this.ApiMessage = new ApiMessage { Message = message };
        }

        public ApiMessage ApiMessage { get; }

        public int StatusCode { get; } = 404;
    }
}
