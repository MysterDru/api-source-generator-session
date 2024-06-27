using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace NorthwindTraders.WebApi.Data;

public static class ServiceCollectionExtensions
{
	public static void ConfigureData(this IServiceCollection services, IConfiguration configuration,
		string connectionStringName = "DefaultConnection")
	{
		services.AddDbContext<NorthwindContext>((sp, opts) =>
		{
			var connectionString = configuration.GetConnectionString(connectionStringName);
			opts.UseSqlServer(connectionString);
#if DEBUG
			opts.EnableSensitiveDataLogging();
#endif
		});

		services.AddTransient<INorthwindRepository, NorthwindContext>();
	}
}
