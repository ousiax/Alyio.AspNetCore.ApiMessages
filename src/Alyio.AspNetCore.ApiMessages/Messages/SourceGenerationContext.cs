using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Alyio.AspNetCore.ApiMessages;

[JsonSerializable(typeof(ProblemDetails))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(List<string>))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}