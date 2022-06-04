using System;

namespace Alyio.AspNetCore.ApiMessages;

/// <summary>
/// Equivalent to HTTP status 500. <see cref="InternalServerErrorMessage"/> indicates that a generic error has occurred on the server.
/// </summary>
public class InternalServerErrorMessage : Exception, IApiMessage
{
    /// <summary>
    /// Initialize a new instance of <see cref="InternalServerErrorMessage"/> class.
    /// </summary>
    public InternalServerErrorMessage() : this(XMessage.InternalServerError)
    {
    }

    /// <summary>
    /// Initialize a new instance of <see cref="InternalServerErrorMessage"/> class.
    /// </summary>
    public InternalServerErrorMessage(string message) : base(message)
    {
        this.ApiMessage = new ApiMessage { Message = message };
    }

    /// <inheritdoc />
    public ApiMessage ApiMessage { get; }

    /// <summary>
    /// 500
    /// </summary>
    public int StatusCode { get; } = 500;
}
