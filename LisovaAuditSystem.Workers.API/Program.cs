using LisovaAuditSystem.Workers.API.Common.Extensions;
using LisovaAuditSystem.Workers.API.Endpoints;

namespace LisovaAuditSystem.Workers.API;

public static class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDataServices(builder.Configuration);
        builder.Services.AddBusinessServices();

        builder.Services.AddAuthorization();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        WebApplication app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapWorkersEndpoints();

        app.Run();
    }
}
