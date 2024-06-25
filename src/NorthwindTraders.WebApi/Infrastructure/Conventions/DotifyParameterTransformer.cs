namespace NorthwindTraders.WebApi.Infrastructure.Conventions;

public class DotifyParameterTransformer : IOutboundParameterTransformer
{
	public string? TransformOutbound(object? value)
	{
		return value?.ToString();
	}
}
