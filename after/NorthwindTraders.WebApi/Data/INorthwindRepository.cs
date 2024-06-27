using Microsoft.EntityFrameworkCore;

namespace NorthwindTraders.WebApi.Data;

public interface INorthwindRepository
{
	public DbSet<TEntity> Set<TEntity>()
		where TEntity : class;

	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
}
