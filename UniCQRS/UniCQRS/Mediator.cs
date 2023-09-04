using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace UniCQRS;

/// <summary>
/// Provides the mediator service for dispatching commands and queries in a CQRS architecture.
/// </summary>
public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<Mediator> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="Mediator"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider for dependency resolution.</param>
    /// <param name="logger">The logger for logging events and errors.</param>
    public Mediator(IServiceProvider serviceProvider, ILogger<Mediator> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task SendAsync(ICommand command)
    {
        await ExecutePipeline(command, async () =>
        {
            var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
            dynamic handler = _serviceProvider.GetService(handlerType);

            if (handler == null)
            {
                _logger.LogError($"No handler found for {command.GetType().Name}");
                throw new InvalidOperationException($"No handler found for {command.GetType().Name}");
            }

            await handler.HandleAsync((dynamic)command);
            return (object)null;
        });
    }

    /// <inheritdoc/>
    public async Task<TResult> SendAsync<TResult>(IQuery<TResult> query)
    {
        return await ExecutePipeline(query, async () =>
        {
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
            dynamic handler = _serviceProvider.GetService(handlerType);

            return await handler.HandleAsync((dynamic)query);
        });
    }

    /// <summary>
    /// Executes the pipeline of behaviors for handling a request.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="request">The request object.</param>
    /// <param name="next">The next function in the pipeline.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the result of the request.</returns>
    private async Task<TResult> ExecutePipeline<TResult>(object request, Func<Task<TResult>> next)
    {
        var behaviors = _serviceProvider.GetServices<IPipelineBehavior<object, TResult>>();
        var pipeline = next;

        foreach (var behavior in behaviors.Reverse())
        {
            pipeline = CreatePipeline(request, behavior, pipeline);
        }

        return await pipeline();
    }

    /// <summary>
    /// Creates a new pipeline function by wrapping a behavior around the next function in the pipeline.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="request">The request object.</param>
    /// <param name="behavior">The pipeline behavior.</param>
    /// <param name="next">The next function in the pipeline.</param>
    /// <returns>A new pipeline function.</returns>
    private Func<Task<TResult>> CreatePipeline<TResult>(object request, IPipelineBehavior<object, TResult> behavior, Func<Task<TResult>> next)
    {
        return async () => await behavior.Handle(request, next);
    }
}
