using System.Text;

using LisovaAuditSystem.Workers.API.Common.Configurations;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace LisovaAuditSystem.Workers.API.Common.Extensions.DependencyInjections;

public static class ApiServicesInjectionExtension
{
    public static void AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<JwtConfiguration>();

        services.AddAuthentication(
                options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
            .AddJwtBearer(
                options =>
                {
                    var jwtConfiguration = new JwtConfiguration(configuration);

                    options.SaveToken = true;

                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,

                            ValidIssuer = jwtConfiguration.Issuer,
                            ValidAudience = jwtConfiguration.Audience,
                            IssuerSigningKey =
                                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.Secret)),

                            RequireExpirationTime = true
                        };

                    options.Events =
                        new JwtBearerEvents
                        {
                            OnMessageReceived =
                                httpContext =>
                                {
                                    string? authorization = httpContext.Request.Headers.Authorization;

                                    if (string.IsNullOrEmpty(authorization))
                                    {
                                        httpContext.NoResult();
                                    }
                                    else
                                    {
                                        httpContext.Token = authorization.Replace("Bearer ", string.Empty);
                                    }

                                    return Task.CompletedTask;
                                }
                        };
                });
        services.AddAuthorization();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }
}
