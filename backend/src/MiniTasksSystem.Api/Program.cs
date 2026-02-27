using MiniTasksSystem.Api;
using MiniTasksSystem.Api.Endpoints.Authentication;
using MiniTasksSystem.Api.Endpoints.Comments;
using MiniTasksSystem.Api.Endpoints.Projects;
using MiniTasksSystem.Api.Endpoints.Tasks;
using MiniTasksSystem.Api.Endpoints.Timeline;
using MiniTasksSystem.Api.Endpoints.Users;
using MiniTasksSystem.Application;
using MiniTasksSystem.Infrastructure;
using MiniTasksSystem.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddPresentation(builder.Configuration);

if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.AddObservability();
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
    await seeder.SeedAsync();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

var api = app.MapGroup("/api");

api.MapGroup("/auth")
    .MapAuthenticationEndpoints()
    .WithTags("Auth");

api.MapGroup("/projects")
    .MapProjectEndpoints()
    .WithTags("Projects")
    .RequireAuthorization();

api.MapGroup("/tasks")
    .MapTaskEndpoints()
    .WithTags("Tasks")
    .RequireAuthorization();

api.MapGroup("/tasks")
    .MapCommentEndpoints()
    .WithTags("Comments")
    .RequireAuthorization();

api.MapGroup("/timeline")
    .MapTimelineEndpoints()
    .WithTags("Timeline")
    .RequireAuthorization();

api.MapGroup("/users")
    .MapUserEndpoints()
    .WithTags("Users")
    .RequireAuthorization();

app.Run();

public partial class Program { }
