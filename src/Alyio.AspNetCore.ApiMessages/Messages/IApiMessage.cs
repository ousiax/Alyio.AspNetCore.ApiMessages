using Microsoft.AspNetCore.Mvc;

namespace Alyio.AspNetCore.ApiMessages;

/// <summary>
/// Represents an <see cref="IApiMessage"/> interface.
/// </summary>
public interface IApiMessage
{
    /// <summary>
    /// Gets the <see cref="ProblemDetails"/>.
    /// </summary>
    ProblemDetails ProblemDetails { get; }
}