using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Alyio.AspNetCore.ApiMessages
{
    /// <summary>
    /// Represents a api message.
    /// </summary>
    public class ApiMessage
    {
        private string _message;

        /// <summary>
        /// Gets or sets the api message.
        /// </summary>
        [JsonProperty("message", Required = Required.Always)]
        public string Message
        {
            get { return _message; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException(nameof(value), "The specified string is null, empty, or consists only of white-space characters.");
                }
                _message = value;
            }
        }

        /// <summary>
        /// Gets or sets a unique identifier to represent this request in trace logs.
        /// </summary>
        /// <seealso cref="Microsoft.AspNetCore.Http.HttpContext.TraceIdentifier"/>
        [JsonProperty("trace_identifier", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string TraceIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the model state errors.
        /// </summary>
        [JsonProperty("errors", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public IList<string> Errors { get; set; }

        /// <summary>
        /// Gets or sets the exception type.
        /// </summary>
        [JsonProperty("exception_type", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string ExceptionType { get; set; }

        /// <summary>
        /// Gets or sets the exception detail information.
        /// </summary>
        [JsonProperty("detail", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string Detail { get; set; }
    }
}
