using System;

namespace Alyio.AspNetCore.ApiMessages
{
    /// <summary>
    /// Equivalent to HTTP status 401. <see cref="UnauthorizedMessage"/> indicates
    /// that the requested resource requires authentication. The WWW-Authenticate header
    /// contains the details of how to perform the authentication.
    /// </summary>
    public sealed class UnauthorizedMessage : Exception, IApiMessage
    {
        public UnauthorizedMessage() : this(XMessage.Unauthorized)
        {
        }

        public UnauthorizedMessage(string message) : base(message)
        {
            this.ApiMessage = new ApiMessage { Message = message };
        }

        public ApiMessage ApiMessage { get; }

        public int StatusCode { get; } = 401;
    }
}
