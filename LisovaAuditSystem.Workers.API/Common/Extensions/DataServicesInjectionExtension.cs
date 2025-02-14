using LisovaAuditSystem.Workers.API.Infrastructure;
using LisovaAuditSystem.Workers.API.Infrastructure.Repository;
using LisovaAuditSystem.Workers.API.Interfaces.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;

namespace LisovaAuditSystem.Workers.API.Common.Extensions;

public static class DataServicesInjectionExtension
{
    public static void AddDataServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<WorkersContext>(
            options =>
                options.UseSqlServer(
                    connectionString: configuration.GetConnectionString("LisovaDb")));

        services.AddScoped<IWorkerRepository, WorkerRepository>();
    }
}
