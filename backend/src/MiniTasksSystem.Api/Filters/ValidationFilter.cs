using FluentValidation;
using MiniTasksSystem.Application.Common.Exceptions;

using AppValidationException = MiniTasksSystem.Application.Common.Exceptions.ValidationException;

namespace MiniTasksSystem.Api.Filters;

public sealed class ValidationFilter<T>(IValidator<T> validator) : IEndpointFilter where T : class
{
    private readonly IValidator<T> _validator = validator;

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var argument = context.Arguments.OfType<T>().FirstOrDefault();

        if (argument is null)
        {
            return await next(context);
        }

        var validationResult = await _validator.ValidateAsync(argument);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new ValidationError(e.PropertyName, e.ErrorMessage));

            throw new AppValidationException(errors);
        }

        return await next(context);
    }
}
