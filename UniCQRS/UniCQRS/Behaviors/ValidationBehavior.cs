using FluentValidation;
using ValidationException = FluentValidation.ValidationException;

namespace UniCQRS;

/// <summary>
/// Provides validation behavior for pipeline requests using FluentValidation.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="validators">A collection of validators for the request type.</param>
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <summary>
    /// Handles the pipeline request, performing validation before passing to the next stage.
    /// </summary>
    /// <param name="request">The request object.</param>
    /// <param name="next">The next function in the pipeline to execute.</param>
    /// <returns>The response for the request.</returns>
    /// <exception cref="ValidationException">Thrown when validation fails.</exception>
    public async Task<TResponse> Handle(TRequest request, Func<Task<TResponse>> next)
    {
        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context)));

        var failures = validationResults
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
        {
            throw new ValidationException("Validation failed", failures);
        }

        return await next();
    }
}
