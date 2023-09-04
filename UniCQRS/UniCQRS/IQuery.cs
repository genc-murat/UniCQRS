namespace UniCQRS;

/// <summary>
/// Marker interface for identifying query objects in a CQRS architecture. 
/// The queries are expected to return a result of the specified type.
/// </summary>
/// <typeparam name="TResult">The type of the result that the query will return.</typeparam>
public interface IQuery<TResult> { }
