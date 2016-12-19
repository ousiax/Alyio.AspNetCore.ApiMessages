using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Alyio.AspNetCore.ApiMessages
{
    public class ApiMessage
    {   /// <summary>
        /// Gets or sets the api message.
        /// </summary>
        [JsonProperty("message", Required = Required.Always)]
        public string Message { get; set; }

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
