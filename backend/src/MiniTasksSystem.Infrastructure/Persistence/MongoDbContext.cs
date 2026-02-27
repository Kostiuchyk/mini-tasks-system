using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MiniTasksSystem.Domain.Projects;
using MiniTasksSystem.Domain.Tasks;
using MiniTasksSystem.Domain.Users;

namespace MiniTasksSystem.Infrastructure.Persistence;

public sealed class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbOptions> options)
    {
        Client = new MongoClient(options.Value.ConnectionString);
        _database = Client.GetDatabase(options.Value.DatabaseName);
    }

    public IMongoClient Client { get; }

    public IMongoCollection<User> Users => _database.GetCollection<User>("users");
    public IMongoCollection<Project> Projects => _database.GetCollection<Project>("projects");
    public IMongoCollection<TaskItem> Tasks => _database.GetCollection<TaskItem>("tasks");
    public IMongoCollection<ProjectTaskCounter> TaskCounters => _database.GetCollection<ProjectTaskCounter>("task_counters");
    public IMongoCollection<Comment> Comments => _database.GetCollection<Comment>("comments");
    public IMongoCollection<AuditLog> AuditLogs => _database.GetCollection<AuditLog>("auditLogs");
    public IMongoCollection<RefreshToken> RefreshTokens => _database.GetCollection<RefreshToken>("refreshTokens");
}
