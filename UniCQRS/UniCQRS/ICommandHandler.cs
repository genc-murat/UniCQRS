namespace UniCQRS;

/// <summary>
/// Defines a handler for processing command objects in a CQRS architecture.
/// </summary>
/// <typeparam name="TCommand">The type of command to handle. Must implement <see cref="ICommand"/>.</typeparam>
public interface ICommandHandler<TCommand> where TCommand : ICommand
{
    /// <summary>
    /// Asynchronously handles the specified command.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    Task HandleAsync(TCommand command);
}
