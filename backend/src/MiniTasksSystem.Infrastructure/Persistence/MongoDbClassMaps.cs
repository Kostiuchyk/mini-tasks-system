using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MiniTasksSystem.Domain.Projects;
using MiniTasksSystem.Domain.Tasks;
using MiniTasksSystem.Domain.Users;

namespace MiniTasksSystem.Infrastructure.Persistence;

internal static class MongoDbClassMaps
{
    public static void Register()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(User)))
        {
            BsonClassMap.RegisterClassMap<User>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id)
                    .SetIdGenerator(StringObjectIdGenerator.Instance)
                    .SetSerializer(new StringSerializer(BsonType.ObjectId));
                cm.MapMember(c => c.Role).SetSerializer(new EnumSerializer<UserRole>(BsonType.String));
            });
        }

        if (!BsonClassMap.IsClassMapRegistered(typeof(Project)))
        {
            BsonClassMap.RegisterClassMap<Project>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id)
                    .SetIdGenerator(StringObjectIdGenerator.Instance)
                    .SetSerializer(new StringSerializer(BsonType.ObjectId));
            });
        }

        if (!BsonClassMap.IsClassMapRegistered(typeof(TaskItem)))
        {
            BsonClassMap.RegisterClassMap<TaskItem>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id)
                    .SetIdGenerator(StringObjectIdGenerator.Instance)
                    .SetSerializer(new StringSerializer(BsonType.ObjectId));
                cm.MapMember(c => c.Status).SetSerializer(new EnumSerializer<TaskItemStatus>(BsonType.String));
                cm.MapMember(c => c.Priority).SetSerializer(new EnumSerializer<TaskItemPriority>(BsonType.String));
            });
        }

        if (!BsonClassMap.IsClassMapRegistered(typeof(Comment)))
        {
            BsonClassMap.RegisterClassMap<Comment>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id)
                    .SetIdGenerator(StringObjectIdGenerator.Instance)
                    .SetSerializer(new StringSerializer(BsonType.ObjectId));
            });
        }

        if (!BsonClassMap.IsClassMapRegistered(typeof(AuditLog)))
        {
            BsonClassMap.RegisterClassMap<AuditLog>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id)
                    .SetIdGenerator(StringObjectIdGenerator.Instance)
                    .SetSerializer(new StringSerializer(BsonType.ObjectId));
            });
        }

        if (!BsonClassMap.IsClassMapRegistered(typeof(RefreshToken)))
        {
            BsonClassMap.RegisterClassMap<RefreshToken>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id)
                    .SetIdGenerator(StringObjectIdGenerator.Instance)
                    .SetSerializer(new StringSerializer(BsonType.ObjectId));
            });
        }

        if (!BsonClassMap.IsClassMapRegistered(typeof(ProjectTaskCounter)))
        {
            BsonClassMap.RegisterClassMap<ProjectTaskCounter>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id);
            });
        }
    }
}
