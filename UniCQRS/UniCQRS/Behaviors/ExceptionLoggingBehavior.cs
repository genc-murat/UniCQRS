using Microsoft.Extensions.Logging;

namespace UniCQRS;

/// <summary>
/// Provides exception logging for pipeline requests.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public class ExceptionLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<ExceptionLoggingBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionLoggingBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="logger">The logger used to log exception information.</param>
    public ExceptionLoggingBehavior(ILogger<ExceptionLoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handles the pipeline request, logging any exceptions that occur.
    /// </summary>
    /// <param name="request">The request object.</param>
    /// <param name="next">The next function in the pipeline to execute.</param>
    /// <returns>The response for the request, or rethrows any caught exceptions.</returns>
    /// <exception cref="Exception">Rethrows any exception caught during processing.</exception>
    public async Task<TResponse> Handle(TRequest request, Func<Task<TResponse>> next)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            // Log the exception and then rethrow it
            _logger.LogError(ex, $"An exception occurred while handling {typeof(TRequest).Name}.");
            throw;
        }
    }
}
