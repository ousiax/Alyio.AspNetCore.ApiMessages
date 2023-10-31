namespace Alyio.AspNetCore.ApiMessages
{
    /// <summary>
    /// A collection of constants for
    /// <see href="https://datatracker.ietf.org/doc/html/rfc7231#section-6">Response Status Codes</see >.
    /// </summary>
    public static class StatusCodeTypes
    {
        /// <summary>
        /// HTTP status code 400.
        /// </summary>
        public const string Status400BadRequest = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1";

        /// <summary>
        /// HTTP status code 401.
        /// </summary>
        public const string Status401Unauthorized = "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1";

        /// <summary>
        /// HTTP status code 403.
        /// </summary>
        public const string Status403Forbidden = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3";

        /// <summary>
        /// HTTP status code 404.
        /// </summary>
        public const string Status404NotFound = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4";

        /// <summary>
        /// HTTP status code 500.
        /// </summary>
        public const string Status500InternalServerError = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1";
    }
}
