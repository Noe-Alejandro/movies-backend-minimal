using System.Reflection;

namespace Movies.WebApi.Endpoints
{
   /// <summary>
   /// Registers all endpoint modules found in the provided assemblies (or in the WebApi assembly if none are provided).
   /// Each module must implement <see cref="IEndpointModule"/>.
   /// </summary>
   public static class EndpointModulesRegister
   {
      /// <summary>
      /// Scans assemblies for <see cref="IEndpointModule"/> implementations, creates them via DI,
      /// and invokes <c>MapEndpoints</c> on each one. Keeps a deterministic registration order.
      /// </summary>
      /// <param name="app">The current <see cref="WebApplication"/>.</param>
      /// <param name="assemblies">
      /// Optional assemblies to scan. If omitted, defaults to the current WebApi assembly.
      /// </param>
      public static void MapEndpointModules(this WebApplication app, params Assembly[] assemblies)
      {
         ArgumentNullException.ThrowIfNull(app);

         var sourceAssemblies = (assemblies is { Length: > 0 })
             ? assemblies
             : new[] { Assembly.GetExecutingAssembly() };

         var moduleTypes = sourceAssemblies
             .SelectMany(a => a.DefinedTypes)
             .Where(t => !t.IsAbstract && !t.IsInterface && typeof(IEndpointModule).IsAssignableFrom(t))
             .OrderBy(t => t.FullName, StringComparer.Ordinal)
             .ToArray();

         foreach (var type in moduleTypes)
         {
            var module = (IEndpointModule)ActivatorUtilities.CreateInstance(app.Services, type.AsType());
            module.MapEndpoints(app);
         }
      }
   }
}
