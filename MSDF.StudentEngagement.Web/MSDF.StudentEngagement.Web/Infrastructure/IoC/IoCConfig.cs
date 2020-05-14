using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace MSDF.StudentEngagement.Web.Infrastructure.IoC
{
    public static class IoCConfig
    {
        public static void RegisterDependencies(IServiceCollection container, IConfiguration configuration)
        {
            // Register other dependencies
            RegisterProviders(container);

            // Register resources/services dependencies
            Resources.Infrastructure.IoC.IoCConfig.RegisterDependencies(container, configuration);
        }

        private static void RegisterProviders(IServiceCollection container)
        {
        }
    }
}
