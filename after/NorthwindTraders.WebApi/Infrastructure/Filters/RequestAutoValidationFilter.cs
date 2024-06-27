using FluentValidation;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NorthwindTraders.WebApi.Infrastructure.Filters;

public sealed class RequestAutoValidationFilter : IAsyncActionFilter
{
	public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
	{
		foreach (var parameter in context.ActionDescriptor.Parameters)
		{
			if (!IsEligibleForValidation(parameter))
			{
				continue;
			}

			var validator = GetValidatorForParameter(parameter, context);
			if (validator == null)
			{
				continue;
			}

			var instance = context.ActionArguments[parameter.Name];
			if (instance == null)
			{
				continue;
			}

			await ValidateParameterInstance(instance, validator, context);
		}

		await next();
	}

	private static bool IsEligibleForValidation(ParameterDescriptor parameter)
	{
		var bindingSource = parameter.BindingInfo?.BindingSource;
		return bindingSource == BindingSource.Body ||
		       (bindingSource == BindingSource.Query && parameter.ParameterType.IsClass);
	}

	private static IValidator? GetValidatorForParameter(ParameterDescriptor parameter, ActionExecutingContext context)
	{
		var validatorType = typeof(IValidator<>).MakeGenericType(parameter.ParameterType);
		return context.HttpContext.RequestServices.GetService(validatorType) as IValidator;
	}

	private static async Task ValidateParameterInstance(object instance, IValidator validator, ActionExecutingContext context)
	{
		var result = await validator.ValidateAsync(new ValidationContext<object>(instance),
			context.HttpContext.RequestAborted);

		if (!result.IsValid)
		{
			foreach (var error in result.Errors)
			{
				context.ModelState.TryAddModelError(error.PropertyName, error.ErrorMessage);
			}
		}
	}
}
