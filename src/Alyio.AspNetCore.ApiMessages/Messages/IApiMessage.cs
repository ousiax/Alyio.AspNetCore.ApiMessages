namespace Alyio.AspNetCore.ApiMessages
{
    /// <summary>
    /// Represents an <see cref="IApiMessage"/> interface.
    /// </summary>
    public interface IApiMessage
    {
        /// <summary>
        /// Gets the  status code defined for HTTP.
        /// </summary>
        int StatusCode { get; }

        /// <summary>
        /// Gets the <see cref="ApiMessage"/>.
        /// </summary>
        ApiMessage ApiMessage { get; }
    }
}
