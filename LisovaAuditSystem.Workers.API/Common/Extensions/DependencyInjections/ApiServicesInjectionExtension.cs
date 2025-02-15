using System.Text;

using LisovaAuditSystem.Workers.API.Common.Configurations;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace LisovaAuditSystem.Workers.API.Common.Extensions.DependencyInjections;

public static class ApiServicesInjectionExtension
{
    public static void AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<JwtConfiguration>();

        services.AddSwaggerGen(
            options =>
            {
                OpenApiSecurityScheme scheme =
                    new()
                    {
                        In = ParameterLocation.Header,
                        Description = "Please enter a valid token",
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        BearerFormat = "JWT",
                        Scheme = "Bearer",
                        Reference =
                            new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                    };

                options.ResolveConflictingActions(apiDesc => apiDesc.First());
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
                options.AddSecurityDefinition(scheme.Reference.Id, scheme);
                options.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        { scheme, Array.Empty<string>() }
                    });
            });

        services.AddCors();

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
