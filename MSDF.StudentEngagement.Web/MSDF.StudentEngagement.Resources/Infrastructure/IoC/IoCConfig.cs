using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MSDF.StudentEngagement.Resources.Providers.Encryption;
using System;
using System.Linq;

namespace MSDF.StudentEngagement.Resources.Infrastructure.IoC
{
    public static class IoCConfig
    {
        public static void RegisterDependencies(IServiceCollection container, IConfiguration configuration)
        {
            // Register persistence dependencies.
            Persistence.Infrastructure.IoC.IoCConfig.RegisterDependencies(container, configuration);

            // Register own dependencies.
            RegisterServicesByConvention<IResourcesMarker>(container);

            RegisterProviders(container);

        }

        private static void RegisterProviders(IServiceCollection container)
        {
        }

        private static void RegisterServicesByConvention<TMarker>(IServiceCollection container)
        {
            var types = typeof(TMarker).Assembly.ExportedTypes;

            var servicesToRegister = (
                from interfaceType in types.Where(t => t.Name.StartsWith("I") && t.Name.EndsWith("Service"))
                from serviceType in types.Where(t => t.Name == interfaceType.Name.Substring(1))
                select new
                {
                    InterfaceType = interfaceType,
                    ServiceType = serviceType
                }
            );

            foreach (var pair in servicesToRegister)
                container.AddScoped(pair.InterfaceType, pair.ServiceType);
        }
    }
}
