using LisovaAuditSystem.Workers.API.Common.Extensions.DependencyInjections;
using LisovaAuditSystem.Workers.API.Endpoints;

namespace LisovaAuditSystem.Workers.API;

public static class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDataServices(builder.Configuration);
        builder.Services.AddBusinessServices();
        builder.Services.AddApiServices(builder.Configuration);

        WebApplication app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseCors();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapWorkersEndpoints();

        app.Run();
    }
}
