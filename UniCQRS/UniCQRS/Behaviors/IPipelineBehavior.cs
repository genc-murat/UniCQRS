namespace UniCQRS;

/// <summary>
/// Defines a behavior for handling pipeline requests and responses.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IPipelineBehavior<TRequest, TResponse>
{
    /// <summary>
    /// Handles a request within the pipeline.
    /// </summary>
    /// <param name="request">The incoming request.</param>
    /// <param name="next">A delegate to the next part of the pipeline.</param>
    /// <returns>An asynchronous task that returns the response.</returns>
    Task<TResponse> Handle(TRequest request, Func<Task<TResponse>> next);
}
