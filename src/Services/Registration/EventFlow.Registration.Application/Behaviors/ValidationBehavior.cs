
namespace EventFlow.Registration.Application.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var vs = validators.ToList();
        if (vs.Count == 0) return await next(ct);
        var c = new ValidationContext<TRequest>(request);
        var r = await Task.WhenAll(vs.Select(v => v.ValidateAsync(c, ct)));
        var e = r.SelectMany(x => x.Errors).Where(x => x is not null).ToList();
        if (e.Count > 0) throw new ValidationException(e);
        return await next(ct);
    }
}
