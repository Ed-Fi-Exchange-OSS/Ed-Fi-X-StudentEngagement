using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MSDF.StudentEngagement.Persistence.EntityFramework;
using System.Linq;
using System.Threading.Tasks;

namespace MSDF.StudentEngagement.Persistence.Infrastructure.IoC
{
    public static class IoCConfig
    {
        public static void RegisterDependencies(IServiceCollection container, IConfiguration configuration)
        {
            // TODO: In the future it would be nice to have an implementation
            //       that can support MsSQL, MySQL, PostreSQL and SQLLite
            // Follow this link to implement different contexts with their own migrations.
            // https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/providers?tabs=dotnet-core-cli

            // For the time being you will have to comment/uncomment the provider you desire
            // and then recreate the migrations.

            //container.AddDbContext<DatabaseContext>(options =>
            //    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            //container.AddDbContext<DatabaseContext>(options =>
            //    options.UseMySQL(configuration.GetConnectionString("DefaultConnection")));

            container.AddDbContext<DatabaseContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            RegisterCommandsAndQueriesByConvention<IPersistenceMarker>(container);
        }

        private static void RegisterCommandsAndQueriesByConvention<TMarker>(IServiceCollection container)
        {
            var types = typeof(TMarker).Assembly.ExportedTypes;

            var commandsToRegister = (
                from interfaceType in types.Where(t => t.Name.StartsWith("I") && t.Name.EndsWith("Commands"))
                from serviceType in types.Where(t => t.Name == interfaceType.Name.Substring(1))
                select new
                {
                    InterfaceType = interfaceType,
                    ServiceType = serviceType
                }
            );

            var queriesToRegister = (
                from interfaceType in types.Where(t => t.Name.StartsWith("I") && t.Name.EndsWith("Queries"))
                from serviceType in types.Where(t => t.Name == interfaceType.Name.Substring(1))
                select new
                {
                    InterfaceType = interfaceType,
                    ServiceType = serviceType
                }
            );

            foreach (var pair in commandsToRegister)
                container.AddScoped(pair.InterfaceType, pair.ServiceType);

            foreach (var pair in queriesToRegister)
                container.AddScoped(pair.InterfaceType, pair.ServiceType);
        }
    }
}
