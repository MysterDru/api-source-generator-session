namespace NorthwindTraders.WebApi.SourceGen;

/// <remarks>
///     This partial class file contains the logic to generate marker interfaces
///     that are used to determine the controller actions to generate.
/// </remarks>
public partial class ApiControllerGenerator
{
    private static void AddMarkerInterfaces(IncrementalGeneratorPostInitializationContext postInitializationContext)
    {
        // define the source of the common interfaces
        var interfacesSource = @"using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NorthwindTraders.WebApi.SourceGen;

public interface IGetHandler<TResponse>
    where TResponse : class
{
    Task<TResponse> GetAsync(int id, CancellationToken cancellationToken);
}

public interface IGetHandler<TArgs, TResponse>
    where TResponse : class
{
    Task<TResponse> GetAsync(TArgs args, CancellationToken cancellationToken);
}

public interface IGetListHandler<TResponse>
    where TResponse : class
{
    Task<IReadOnlyList<TResponse>> GetListAsync(CancellationToken cancellationToken);
}

public interface IGetListHandler<TArgs, TResponse> where TResponse : class
{
    Task<IReadOnlyList<TResponse>> GetListAsync(TArgs args, CancellationToken cancellationToken);
}

public interface ICreateHandler<in TRequest, TResponse> : IGetHandler<TResponse>
    where TRequest : class
    where TResponse : class
{
    Task<int> CreateAsync(TRequest request, CancellationToken cancellationToken);
}

public interface IUpdateHandler<in TRequest>
    where TRequest : class
{
    Task UpdateAsync(int id, TRequest request, CancellationToken cancellationToken);
}

public interface IUpdateHandler<TArgs, in TRequest>
    where TRequest : class
{
    Task UpdateAsync(TArgs args, TRequest request, CancellationToken cancellationToken);
}

public interface IDeleteHandler
{
    Task DeleteAsync(int id, CancellationToken cancellationToken);
}

public interface IDeleteHandler<TArgs>
{
    Task DeleteAsync(TArgs id, CancellationToken cancellationToken);
}

public interface ISaveHandler<in TRequest>
    where TRequest : class
{
    Task SaveAsync(TRequest request, CancellationToken cancellationToken);
}";

        var hintName = "NorthwindTraders.WebApi.SourceGen.Interfaces.g.cs";

        // add the source to the application
        postInitializationContext.AddSource(hintName, interfacesSource);
    }
}