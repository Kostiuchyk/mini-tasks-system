namespace MiniTasksSystem.Infrastructure.Persistence;

public sealed class MongoDbOptions
{
    public const string SectionName = "MongoDb";

    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
}
