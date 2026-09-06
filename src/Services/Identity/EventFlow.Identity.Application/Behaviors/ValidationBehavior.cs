// Runs FluentValidation validators before a MediatR handler executes.
using FluentValidation;
using MediatR;

namespace EventFlow.Identity.Application.Behaviors
{
    public sealed class ValidationBehavior<TRequest , TResponse> : IPipelineBehavior<TRequest , TResponse> where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // If there is no validator for this request,
            // continue directly to the handler.
            if (!_validators.Any())
            {
                return await next();
            }

            // Create FluentValidation context.
            var context = new ValidationContext<TRequest>(request);

            // Execute all validators.
            var results = await Task.WhenAll(
                _validators.Select(validator => validator.ValidateAsync(context,cancellationToken)));

            // Collect validation failures.
            var failures = results
                .SelectMany(result => result.Errors)
                .Where(error => error is not null)
                .ToList();

            // Stop execution when validation fails.
            if (failures.Count > 0)
            {
                throw new ValidationException(failures);
            }

            // Continue to the command/query handler.
            return await next();
        }
    }
}
