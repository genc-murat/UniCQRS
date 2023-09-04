using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace UniCQRS;

/// <summary>
/// Provides timing behavior for pipeline requests, logging the elapsed time for handling each request.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public class TimingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<TimingBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimingBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="logger">The logger used to log timing information.</param>
    public TimingBehavior(ILogger<TimingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handles the pipeline request, measuring and logging the time it takes for the request to be handled.
    /// </summary>
    /// <param name="request">The request object.</param>
    /// <param name="next">The next function in the pipeline to execute.</param>
    /// <returns>The response for the request.</returns>
    public async Task<TResponse> Handle(TRequest request, Func<Task<TResponse>> next)
    {
        var sw = Stopwatch.StartNew();
        var response = await next();
        sw.Stop();

        _logger.LogInformation($"Handling {typeof(TRequest).Name} took {sw.ElapsedMilliseconds} ms.");

        return response;
    }
}
