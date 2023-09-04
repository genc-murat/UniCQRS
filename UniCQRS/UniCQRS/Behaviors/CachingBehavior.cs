using System.Runtime.Caching;

namespace UniCQRS;

/// <summary>
/// Provides caching behavior for pipeline requests.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ObjectCache _cache;
    private readonly TimeSpan _cacheDuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="CachingBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="cache">The object cache to use.</param>
    /// <param name="cacheDuration">The duration to keep items in the cache.</param>
    public CachingBehavior(ObjectCache cache, TimeSpan cacheDuration)
    {
        _cache = cache;
        _cacheDuration = cacheDuration;
    }

    /// <summary>
    /// Handles the pipeline request, fetching from cache if available.
    /// </summary>
    /// <param name="request">The request object.</param>
    /// <param name="next">The next function in the pipeline to execute if the cache is not hit.</param>
    /// <returns>The response for the request, either from cache or from executing the next function in the pipeline.</returns>
    public async Task<TResponse> Handle(TRequest request, Func<Task<TResponse>> next)
    {
        // If the request is a query, try to get its result from cache
        if (request is IQuery<TResponse>)
        {
            var cacheKey = request.GetHashCode().ToString();
            var cachedResponse = (TResponse)_cache[cacheKey];

            if (cachedResponse != null)
            {
                return cachedResponse;
            }

            // Execute the next function in the pipeline and cache its result
            var response = await next();
            _cache.Add(cacheKey, response, DateTimeOffset.UtcNow.Add(_cacheDuration));
            return response;
        }

        // If it's not a query, just execute the next function in the pipeline
        return await next();
    }
}
