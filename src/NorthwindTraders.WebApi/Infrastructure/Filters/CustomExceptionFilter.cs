using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using NorthwindTraders.WebApi.Infrastructure.Exceptions;

namespace NorthwindTraders.WebApi.Infrastructure.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class CustomExceptionFilterAttribute(ProblemDetailsFactory problemDetailsFactory) : ExceptionFilterAttribute
{
	public override void OnException(ExceptionContext context)
	{
		var code = context.Exception switch
		{
			NotFoundException => HttpStatusCode.NotFound,
			DeleteFailureException => HttpStatusCode.Conflict,
			_ => HttpStatusCode.InternalServerError
		};

		var message = context.Exception switch
		{
			NotFoundException notFound => notFound.Message,
			DeleteFailureException deleteFailure => deleteFailure.Message,
			_ => "An internal error occurred"
		};

		var title = context.Exception.GetType().FullName;
		var details = problemDetailsFactory.CreateProblemDetails(context.HttpContext, (int)code, null, null, message, null);

		// context.re

		context.HttpContext.Response.ContentType = "application/json";
		context.HttpContext.Response.StatusCode = (int)code;
		context.Result = new JsonResult(details);
	}
}
