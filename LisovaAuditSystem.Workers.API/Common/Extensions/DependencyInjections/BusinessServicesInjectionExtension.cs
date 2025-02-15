﻿using LisovaAuditSystem.Workers.API.Interfaces.Services;
using LisovaAuditSystem.Workers.API.Services;
using LisovaAuditSystem.Workers.API.Services.Authentication;

namespace LisovaAuditSystem.Workers.API.Common.Extensions.DependencyInjections;

public static class BusinessServicesInjectionExtension
{
    public static void AddBusinessServices(this IServiceCollection services)
    {
        services.AddScoped<IWorkerService, WorkerService>();

        services.AddTransient<JwtTokenGenerationService>();
    }
}
