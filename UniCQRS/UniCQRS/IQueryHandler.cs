namespace UniCQRS;

/// <summary>
/// Defines the contract for handling query objects in a CQRS architecture.
/// </summary>
/// <typeparam name="TQuery">The type of the query to handle. Must implement <see cref="IQuery{TResult}"/>.</typeparam>
/// <typeparam name="TResult">The type of the result returned by the query.</typeparam>
public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
{
    /// <summary>
    /// Asynchronously handles the specified query and retrieves the result.
    /// </summary>
    /// <param name="query">The query object to handle.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the result of the query.</returns>
    Task<TResult> HandleAsync(TQuery query);
}
