using FluentValidation;
using Microsoft.Extensions.Compliance.Classification;
using Microsoft.Extensions.Compliance.Redaction;
using Microsoft.OpenApi.Models;
using MiniTasksSystem.Api.Authorization;
using MiniTasksSystem.Api.Middleware;
using MiniTasksSystem.Application.Compliance;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using CorsOptions = MiniTasksSystem.Api.Settings.CorsOptions;
using OpenTelemetryOptions = MiniTasksSystem.Api.Settings.OpenTelemetryOptions;

namespace MiniTasksSystem.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddValidation();
        services.AddExceptionHandling();
        services.AddPolicies();
        services.AddSwagger();
        services.AddCorsPolicy(configuration);
        services.AddComplianceRedaction(configuration);

        return services;
    }

    private static void AddValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);
        services.AddHttpContextAccessor();
    }

    private static void AddExceptionHandling(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
    }

    private static void AddPolicies(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(PolicyNames.AdminOnly, policy => policy.RequireRole("Admin"));
    }

    private static void AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }

    public static WebApplicationBuilder AddObservability(this WebApplicationBuilder builder)
    {
        OpenTelemetryOptions options = builder.Configuration.GetSection(OpenTelemetryOptions.SectionName).Get<OpenTelemetryOptions>()!;

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(options.ServiceName))
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation())
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation())
            .UseOtlpExporter(OtlpExportProtocol.HttpProtobuf, new Uri(options.Endpoint));

        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeScopes = true;
            logging.IncludeFormattedMessage = true;
        });

        builder.Logging.EnableRedaction();

        return builder;
    }

    private static void AddComplianceRedaction(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRedaction(redaction =>
        {
            redaction.SetHmacRedactor(
                configuration.GetSection("Compliance:Hmac"),
                new DataClassificationSet(DataTaxonomy.PiiData));

            redaction.SetRedactor<ErasingRedactor>(
                new DataClassificationSet(DataTaxonomy.SensitiveData));

            redaction.SetFallbackRedactor<ErasingRedactor>();
        });
    }

    private static void AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
    {
        CorsOptions corsOptions = configuration.GetSection(CorsOptions.SectionName).Get<CorsOptions>()!;

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins(corsOptions.AllowedOrigins)
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });
    }
}