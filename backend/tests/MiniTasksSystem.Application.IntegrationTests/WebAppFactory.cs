using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Testcontainers.MongoDb;
using Xunit;

namespace MiniTasksSystem.Application.IntegrationTests;

public sealed class WebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MongoDbContainer _mongoContainer = new MongoDbBuilder()
        .WithImage("mongo:7")
        .WithReplicaSet()
        .Build();

    public async Task InitializeAsync()
    {
        await _mongoContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _mongoContainer.StopAsync();
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["MongoDb:ConnectionString"] = _mongoContainer.GetConnectionString(),
                ["MongoDb:DatabaseName"] = "integration_tests",
                ["Jwt:Secret"] = "SuperSecretKeyForTestingPurposesOnly!!",
                ["Jwt:Issuer"] = "MiniTasksSystem",
                ["Jwt:Audience"] = "MiniTasksSystem",
                ["Jwt:ExpirationMinutes"] = "60",
                ["Cors:AllowedOrigins:0"] = "http://localhost:3000",
                ["Compliance:Hmac:Key"] = "YWJjZGVmZ2hpamtsbW5vcHFyc3R1dnd4eXoxMjM0NTY=",
                ["Compliance:Hmac:KeyId"] = "1"
            });
        });
    }
}
