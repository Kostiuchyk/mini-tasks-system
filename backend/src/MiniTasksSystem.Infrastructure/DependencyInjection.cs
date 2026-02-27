using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MiniTasksSystem.Application.Authentication;
using MiniTasksSystem.Application.Comments;
using MiniTasksSystem.Application.Common;
using MiniTasksSystem.Application.Projects;
using MiniTasksSystem.Application.Tasks;
using MiniTasksSystem.Application.Timeline;
using MiniTasksSystem.Application.Users;
using MiniTasksSystem.Infrastructure.Authentication;
using MiniTasksSystem.Infrastructure.Persistence;
using MiniTasksSystem.Infrastructure.Repositories;

namespace MiniTasksSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        MongoDbClassMaps.Register();

        services.AddOptions<MongoDbOptions>().BindConfiguration(MongoDbOptions.SectionName);
        services.AddSingleton<MongoDbContext>();
        services.AddScoped<MongoSessionAccessor>();
        services.AddScoped<IUnitOfWork, MongoUnitOfWork>();

        var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()!;
        services.AddOptions<JwtOptions>().BindConfiguration(JwtOptions.SectionName);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization();

        services.AddSingleton<JwtProvider>();
        services.AddScoped<BcryptPasswordHasher>();
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<ITaskItemRepository, TaskItemRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        services.AddScoped<IDataSeeder, DataSeeder>();

        return services;
    }
}
