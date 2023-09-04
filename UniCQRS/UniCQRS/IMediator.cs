namespace UniCQRS;

/// <summary>
/// Defines the contract for a mediator service in a CQRS architecture, responsible for dispatching commands and queries.
/// </summary>
public interface IMediator
{
    /// <summary>
    /// Asynchronously sends a command for handling.
    /// </summary>
    /// <param name="command">The command object implementing <see cref="ICommand"/>.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task SendAsync(ICommand command);

    /// <summary>
    /// Asynchronously sends a query for handling and retrieves the result.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="query">The query object implementing <see cref="IQuery{TResult}"/>.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the result of the query.</returns>
    Task<TResult> SendAsync<TResult>(IQuery<TResult> query);
}
