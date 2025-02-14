using LisovaAuditSystem.Workers.API.Services;

namespace LisovaAuditSystem.Workers.API.Common.Extensions;

public static class BusinessServicesInjectionExtension
{
    public static void AddBusinessServices(this IServiceCollection services)
    {
        services.AddScoped<IWorkerService, WorkerService>();
    }
}
