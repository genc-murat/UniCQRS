# UniCQRS

UniCQRS is a lightweight library designed to provide essential building blocks for implementing the Command Query Responsibility Segregation (CQRS) pattern in .NET applications. The library offers interfaces and concrete implementations for Commands, Queries, Handlers, and Pipeline Behaviors, along with exception handling, validation, caching, and logging utilities.

## Features

- **Commands and Queries**:
  - `ICommand` and `IQuery<TResult>` interfaces for defining commands and queries.
  
- **Handlers**:
  - `ICommandHandler<TCommand>` and `IQueryHandler<TQuery, TResult>` interfaces for implementing business logic.
  
- **Pipeline Behaviors**:
  - Interceptors that execute pre- and post- request handling. For example, logging, validation, and caching behaviors.
  
- **Validation**:
  - Integration with FluentValidation for request validation.
  
- **Caching**:
  - Support for caching query responses.
  
- **Exception Handling and Logging**:
  - Interceptors for logging exceptions that occur during request handling.

## Installation

You can install the library via NuGet package manager:

```bash
Install-Package UniCQRS
```

## Usage

Below is a basic guide on how to use the library features:

### 1. Define a Command

```csharp
public class CreateUser : ICommand
{
    public string Username { get; set; }
    public string Password { get; set; }
}
```

### 2. Implement Command Handler

```csharp
public class CreateUserHandler : ICommandHandler<CreateUser>
{
    public async Task HandleAsync(CreateUser command)
    {
        // Your business logic here
    }
}
```

### 3. Define a Query

```csharp
public class GetUserById : IQuery<User>
{
    public int Id { get; set; }
}
```

### 4. Implement Query Handler

```csharp
public class GetUserByIdHandler : IQueryHandler<GetUserById, User>
{
    public async Task<User> HandleAsync(GetUserById query)
    {
        // Your business logic here
    }
}
```

### 5. Use Mediator to Send Commands and Queries

```csharp
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> CreateUser(CreateUser command)
    {
        await _mediator.SendAsync(command);
        return Ok();
    }

    public async Task<IActionResult> GetUserById(int id)
    {
        var query = new GetUserById { Id = id };
        var user = await _mediator.SendAsync(query);
        return Ok(user);
    }
}
```

### 6. Implement Custom Pipeline Behaviors

You can add your custom behaviors to the pipeline. Below are examples of how to implement some of the built-in behaviors like Timing, Exception Logging, Validation, and Caching.

#### Timing Behavior

Logs the time taken to handle a request.

```csharp
public class TimingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<TimingBehavior<TRequest, TResponse>> _logger;

    public TimingBehavior(ILogger<TimingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, Func<Task<TResponse>> next)
    {
        var sw = Stopwatch.StartNew();
        var response = await next();
        sw.Stop();
        _logger.LogInformation($"Handling {typeof(TRequest).Name} took {sw.ElapsedMilliseconds} ms.");
        return response;
    }
}
```

#### Exception Logging Behavior

Logs any exceptions that occur during request handling.

```csharp
public class ExceptionLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<ExceptionLoggingBehavior<TRequest, TResponse>> _logger;

    public ExceptionLoggingBehavior(ILogger<ExceptionLoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, Func<Task<TResponse>> next)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An exception occurred while handling {typeof(TRequest).Name}.");
            throw;
        }
    }
}
```

#### Validation Behavior

Validates the request using FluentValidation.

```csharp
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, Func<Task<TResponse>> next)
    {
        // Validation logic here
    }
}
```

#### Caching Behavior

Caches the query response.

```csharp
public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ObjectCache _cache;
    private readonly TimeSpan _cacheDuration;

    public CachingBehavior(ObjectCache cache, TimeSpan cacheDuration)
    {
        _cache = cache;
        _cacheDuration = cacheDuration;
    }

    public async Task<TResponse> Handle(TRequest request, Func<Task<TResponse>> next)
    {
        // Caching logic here
    }
}
```

### 7. Registering Behaviors

To use these behaviors, you will need to register them with your dependency injection container.

```csharp
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TimingBehavior<,>));
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionLoggingBehavior<,>));
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
```

Now, these behaviors will be applied in the order they are registered, whenever you use the `IMediator` to send a command or query.

For complete customization, you can implement your own behaviors that implement `IPipelineBehavior<TRequest, TResponse>` and register them the same way.
