using MongoDB.Driver;

namespace MiniTasksSystem.Infrastructure.Persistence;

internal sealed class MongoSessionAccessor
{
    public IClientSessionHandle? Session { get; set; }
}
