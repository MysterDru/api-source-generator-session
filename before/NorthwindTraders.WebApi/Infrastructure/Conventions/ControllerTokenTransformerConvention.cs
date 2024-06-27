using Humanizer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace NorthwindTraders.WebApi.Infrastructure.Conventions;

public class ControllerTokenTransformerConvention : IActionModelConvention
{
    public void Apply(ActionModel action)
    {
        action.RouteParameterTransformer = new ControllerTokenTransformer(action.Controller.ControllerName);
    }

    private class ControllerTokenTransformer(string controller) : IOutboundParameterTransformer
    {
        public string? TransformOutbound(object? value)
        {
            var valueString = value?.ToString();
            if (valueString != controller) return valueString;

            var dotNotation = valueString.Underscore().Replace('_', '.');

            return dotNotation;
        }
    }
}