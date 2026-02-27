using MongoDB.Bson;
using MongoDB.Driver;
using MiniTasksSystem.Domain.Projects;
using MiniTasksSystem.Domain.Tasks;
using MiniTasksSystem.Domain.Users;
using MiniTasksSystem.Infrastructure.Authentication;

namespace MiniTasksSystem.Infrastructure.Persistence;

public interface IDataSeeder
{
    Task SeedAsync();
}

internal sealed class DataSeeder(MongoDbContext context, BcryptPasswordHasher passwordHasher) : IDataSeeder
{
    private readonly MongoDbContext _context = context;
    private readonly BcryptPasswordHasher _passwordHasher = passwordHasher;

    public async Task SeedAsync()
    {
        var existingUsers = await _context.Users.CountDocumentsAsync(FilterDefinition<User>.Empty);
        if (existingUsers > 0)
            return;

        // --- Users ---
        var u1 = ObjectId.GenerateNewId().ToString();
        var u2 = ObjectId.GenerateNewId().ToString();

        List<User> users =
        [
            new()
            {
                Id = u1,
                FullName = "Admin User",
                Email = "admin@gmail.com",
                PasswordHash = _passwordHasher.Hash("admin123"),
                Role = UserRole.Admin,
                CreatedAt = DateTime.Parse("2026-01-01T08:00:00Z").ToUniversalTime()
            },
            new()
            {
                Id = u2,
                FullName = "Member User",
                Email = "member@gmail.com",
                PasswordHash = _passwordHasher.Hash("member123"),
                Role = UserRole.Member,
                CreatedAt = DateTime.Parse("2026-01-01T08:00:00Z").ToUniversalTime()
            }
        ];

        await _context.Users.InsertManyAsync(users);

        // --- Projects ---
        var p1 = ObjectId.GenerateNewId().ToString();
        var p2 = ObjectId.GenerateNewId().ToString();
        var p3 = ObjectId.GenerateNewId().ToString();

        List<Project> projects =
        [
            new()
            {
                Id = p1,
                Name = "Website Redesign",
                Description = "Complete redesign of the company website with new branding",
                CreatedAt = DateTime.Parse("2026-01-10T09:00:00Z").ToUniversalTime()
            },
            new()
            {
                Id = p2,
                Name = "Mobile App",
                Description = "Build cross-platform mobile application v1",
                CreatedAt = DateTime.Parse("2026-01-20T10:00:00Z").ToUniversalTime()
            },
            new()
            {
                Id = p3,
                Name = "API Platform",
                Description = "Design and implement the public API platform",
                CreatedAt = DateTime.Parse("2026-02-01T08:00:00Z").ToUniversalTime()
            }
        ];

        await _context.Projects.InsertManyAsync(projects);

        // --- Tasks ---
        // Mapping helpers: assignee lookup
        string? assignee(string? key) => key switch
        {
            "u1" => u1,
            "u2" => u2,
            _ => null
        };

        string project(string key) => key switch
        {
            "p1" => p1,
            "p2" => p2,
            "p3" => p3,
            _ => throw new InvalidOperationException($"Unknown project key: {key}")
        };

        // Status mapping: frontend "todo" → New, "inProgress" → Active, "done" → Done
        TaskItemStatus status(string s) => s switch
        {
            "todo" => TaskItemStatus.New,
            "inProgress" => TaskItemStatus.Active,
            "done" => TaskItemStatus.Done,
            _ => throw new InvalidOperationException($"Unknown status: {s}")
        };

        TaskItemPriority priority(string p) => Enum.Parse<TaskItemPriority>(p, ignoreCase: true);

        // Generate task IDs and store mapping for comments/audit logs
        var taskIds = new Dictionary<string, string>();
        for (var i = 1; i <= 30; i++)
            taskIds[$"t{i}"] = ObjectId.GenerateNewId().ToString();

        // Project 1: Website Redesign (5 tasks, numbers 1-5)
        // Project 2: Mobile App (10 tasks, numbers 1-10)
        // Project 3: API Platform (15 tasks, numbers 1-15)
        var taskData = new (string key, string title, string description, string status, string priority, string? assigneeKey, string projectKey, int number, string createdAt, string updatedAt)[]
        {
            // Project 1
            ("t1",  "Create wireframes",             "Design wireframes for all main pages",                       "done",       "high",   "u1", "p1",  1, "2026-01-11T09:00:00Z", "2026-01-18T14:00:00Z"),
            ("t2",  "Design color palette",          "Choose new brand colors and create palette",                 "done",       "medium", "u2", "p1",  2, "2026-01-11T10:00:00Z", "2026-01-15T11:00:00Z"),
            ("t3",  "Build homepage",                "Implement the homepage layout and components",               "inProgress", "high",   "u1", "p1",  3, "2026-01-14T09:00:00Z", "2026-02-10T16:00:00Z"),
            ("t4",  "Setup CI/CD pipeline",          "Configure deployment pipeline for the website",              "todo",       "low",    null, "p1",  4, "2026-01-16T08:00:00Z", "2026-01-16T08:00:00Z"),
            ("t5",  "Write content for About page",  "Draft text and gather images for the About section",        "todo",       "medium", "u2", "p1",  5, "2026-01-20T10:00:00Z", "2026-01-20T10:00:00Z"),

            // Project 2
            ("t6",  "Setup React Native project",    "Initialize project with Expo and configure tooling",        "done",       "high",   "u1", "p2",  1, "2026-01-21T09:00:00Z", "2026-01-23T12:00:00Z"),
            ("t7",  "Design login screen",           "Create UI for login and registration flow",                  "done",       "high",   "u2", "p2",  2, "2026-01-21T10:00:00Z", "2026-01-28T15:00:00Z"),
            ("t8",  "Implement auth flow",           "Connect login screen to authentication API",                 "inProgress", "high",   "u1", "p2",  3, "2026-01-24T09:00:00Z", "2026-02-05T10:00:00Z"),
            ("t9",  "Build dashboard screen",        "Main screen showing user overview and stats",                "inProgress", "medium", "u2", "p2",  4, "2026-01-25T11:00:00Z", "2026-02-08T09:00:00Z"),
            ("t10", "Add push notifications",        "Integrate push notification service",                        "todo",       "medium", "u1", "p2",  5, "2026-01-27T08:00:00Z", "2026-01-27T08:00:00Z"),
            ("t11", "Create settings page",          "User settings and preferences screen",                       "todo",       "low",    null, "p2",  6, "2026-01-28T09:00:00Z", "2026-01-28T09:00:00Z"),
            ("t12", "Implement offline mode",        "Cache data locally for offline access",                      "todo",       "high",   "u1", "p2",  7, "2026-01-30T10:00:00Z", "2026-01-30T10:00:00Z"),
            ("t13", "Write unit tests",              "Add tests for core business logic",                          "todo",       "medium", "u2", "p2",  8, "2026-02-01T09:00:00Z", "2026-02-01T09:00:00Z"),
            ("t14", "Performance optimization",      "Profile and optimize list rendering",                        "todo",       "low",    null, "p2",  9, "2026-02-03T08:00:00Z", "2026-02-03T08:00:00Z"),
            ("t15", "App store submission",          "Prepare assets and submit to stores",                        "todo",       "low",    "u1", "p2", 10, "2026-02-05T09:00:00Z", "2026-02-05T09:00:00Z"),

            // Project 3
            ("t16", "Define API spec",               "Write OpenAPI specification for all endpoints",              "done",       "high",   "u1", "p3",  1, "2026-02-02T09:00:00Z", "2026-02-06T14:00:00Z"),
            ("t17", "Setup .NET project",            "Initialize ASP.NET Core project with folder structure",      "done",       "high",   "u1", "p3",  2, "2026-02-02T10:00:00Z", "2026-02-04T11:00:00Z"),
            ("t18", "Configure MongoDB",             "Setup MongoDB connection and data access layer",             "done",       "high",   "u2", "p3",  3, "2026-02-03T09:00:00Z", "2026-02-07T16:00:00Z"),
            ("t19", "Implement auth endpoints",      "JWT-based authentication with refresh tokens",               "inProgress", "high",   "u1", "p3",  4, "2026-02-05T09:00:00Z", "2026-02-15T10:00:00Z"),
            ("t20", "Build CRUD for projects",       "REST endpoints for project management",                      "inProgress", "medium", "u2", "p3",  5, "2026-02-06T10:00:00Z", "2026-02-14T09:00:00Z"),
            ("t21", "Build CRUD for tasks",          "REST endpoints for task management with filters",            "inProgress", "medium", "u1", "p3",  6, "2026-02-07T09:00:00Z", "2026-02-16T11:00:00Z"),
            ("t22", "Add rate limiting",             "Implement API rate limiting middleware",                      "todo",       "medium", null, "p3",  7, "2026-02-08T08:00:00Z", "2026-02-08T08:00:00Z"),
            ("t23", "Setup logging",                 "Structured logging with Serilog",                            "todo",       "low",    "u2", "p3",  8, "2026-02-09T09:00:00Z", "2026-02-09T09:00:00Z"),
            ("t24", "Write integration tests",       "Test API endpoints with test database",                      "todo",       "medium", "u1", "p3",  9, "2026-02-10T10:00:00Z", "2026-02-10T10:00:00Z"),
            ("t25", "API documentation page",        "Auto-generate Swagger docs from OpenAPI spec",               "todo",       "low",    null, "p3", 10, "2026-02-11T08:00:00Z", "2026-02-11T08:00:00Z"),
            ("t26", "Implement webhooks",            "Allow external services to subscribe to events",             "todo",       "high",   "u1", "p3", 11, "2026-02-12T09:00:00Z", "2026-02-12T09:00:00Z"),
            ("t27", "Add caching layer",             "Redis caching for frequently accessed data",                 "todo",       "medium", "u2", "p3", 12, "2026-02-13T10:00:00Z", "2026-02-13T10:00:00Z"),
            ("t28", "Health check endpoint",         "Implement /health for monitoring",                           "todo",       "low",    null, "p3", 13, "2026-02-14T08:00:00Z", "2026-02-14T08:00:00Z"),
            ("t29", "Versioning strategy",           "Implement API versioning via URL path",                      "todo",       "medium", "u1", "p3", 14, "2026-02-15T09:00:00Z", "2026-02-15T09:00:00Z"),
            ("t30", "Deploy to staging",             "Setup Docker and deploy to staging environment",             "todo",       "high",   "u2", "p3", 15, "2026-02-16T10:00:00Z", "2026-02-16T10:00:00Z"),
        };

        var tasks = taskData.Select(t => new TaskItem
        {
            Id = taskIds[t.key],
            Number = t.number,
            Title = t.title,
            Description = t.description,
            Status = status(t.status),
            Priority = priority(t.priority),
            ProjectId = project(t.projectKey),
            AssigneeId = assignee(t.assigneeKey),
            CreatedAt = DateTime.Parse(t.createdAt).ToUniversalTime(),
            UpdatedAt = DateTime.Parse(t.updatedAt).ToUniversalTime()
        }).ToList();

        await _context.Tasks.InsertManyAsync(tasks);

        // --- Task Counters ---
        List<ProjectTaskCounter> counters =
        [
            new() { Id = p1, Seq = 5 },
            new() { Id = p2, Seq = 10 },
            new() { Id = p3, Seq = 15 }
        ];

        await _context.TaskCounters.InsertManyAsync(counters);

        // --- Comments ---
        // Backend: { Id, TaskId, AuthorId, Text, CreatedAt }
        var commentData = new (string taskKey, string text, string authorKey, string createdAt)[]
        {
            ("t1",  "Wireframes look great, approved by the team.",          "u2", "2026-01-15T14:00:00Z"),
            ("t3",  "Should we use the new grid system for the layout?",     "u2", "2026-02-08T10:00:00Z"),
            ("t3",  "Yes, let's go with CSS Grid. It's cleaner.",            "u1", "2026-02-08T11:30:00Z"),
            ("t7",  "Can we add social login buttons too?",                  "u1", "2026-01-26T09:00:00Z"),
            ("t8",  "Token refresh is working but needs error handling.",     "u1", "2026-02-03T15:00:00Z"),
            ("t16", "Spec reviewed and approved.",                            "u2", "2026-02-05T16:00:00Z"),
            ("t18", "Using MongoDB driver directly, no ODM.",                "u2", "2026-02-06T10:00:00Z"),
            ("t19", "Refresh token rotation is implemented.",                "u1", "2026-02-14T11:00:00Z"),
            ("t21", "Filters for status and priority are done, search is WIP.", "u1", "2026-02-15T09:00:00Z"),
            ("t26", "Should we use a message queue for webhook delivery?",   "u2", "2026-02-13T14:00:00Z"),
        };

        var comments = commentData.Select(c => new Comment
        {
            Id = ObjectId.GenerateNewId().ToString(),
            TaskId = taskIds[c.taskKey],
            AuthorId = assignee(c.authorKey)!,
            Text = c.text,
            CreatedAt = DateTime.Parse(c.createdAt).ToUniversalTime()
        }).ToList();

        await _context.Comments.InsertManyAsync(comments);

        // --- Audit Logs ---
        // Backend: { Id, TaskId, UserId, Action, OldValue?, NewValue?, CreatedAt }
        var auditData = new (string taskKey, string action, string? oldValue, string? newValue, string userKey, string timestamp)[]
        {
            // Task t1 — created → done
            ("t1", "created",        null,            "Create wireframes",                     "u1", "2026-01-11T09:00:00Z"),
            ("t1", "status_changed", "New",           "Active",                                "u1", "2026-01-13T10:00:00Z"),
            ("t1", "status_changed", "Active",        "Done",                                  "u1", "2026-01-18T14:00:00Z"),

            // Task t2 — created → done
            ("t2", "created",        null,            "Design color palette",                  "u2", "2026-01-11T10:00:00Z"),
            ("t2", "status_changed", "New",           "Done",                                  "u2", "2026-01-15T11:00:00Z"),

            // Task t3 — created → inProgress, priority changed
            ("t3", "created",          null,          "Build homepage",                        "u1", "2026-01-14T09:00:00Z"),
            ("t3", "status_changed",   "New",         "Active",                                "u1", "2026-02-01T09:00:00Z"),
            ("t3", "priority_changed", "Medium",      "High",                                  "u2", "2026-02-10T16:00:00Z"),

            // Tasks t4, t5 — just created
            ("t4", "created", null, "Setup CI/CD pipeline",          "u1", "2026-01-16T08:00:00Z"),
            ("t5", "created", null, "Write content for About page",  "u2", "2026-01-20T10:00:00Z"),

            // Task t6 — created → done
            ("t6", "created",        null,    "Setup React Native project", "u1", "2026-01-21T09:00:00Z"),
            ("t6", "status_changed", "New",   "Done",                       "u1", "2026-01-23T12:00:00Z"),

            // Task t7 — created → done
            ("t7", "created",        null,      "Design login screen", "u2", "2026-01-21T10:00:00Z"),
            ("t7", "status_changed", "New",     "Active",              "u2", "2026-01-24T09:00:00Z"),
            ("t7", "status_changed", "Active",  "Done",                "u2", "2026-01-28T15:00:00Z"),

            // Task t8 — created → inProgress
            ("t8", "created",        null,  "Implement auth flow", "u1", "2026-01-24T09:00:00Z"),
            ("t8", "status_changed", "New", "Active",              "u1", "2026-02-02T10:00:00Z"),

            // Task t9 — created → assignee changed → inProgress
            ("t9", "created",          null, "Build dashboard screen", "u1", "2026-01-25T11:00:00Z"),
            ("t9", "assignee_changed", "u1", "u2",                    "u1", "2026-02-06T09:00:00Z"),
            ("t9", "status_changed",   "New", "Active",               "u2", "2026-02-08T09:00:00Z"),

            // Tasks t10–t15 — just created
            ("t10", "created", null, "Add push notifications",    "u1", "2026-01-27T08:00:00Z"),
            ("t11", "created", null, "Create settings page",      "u2", "2026-01-28T09:00:00Z"),
            ("t12", "created", null, "Implement offline mode",    "u1", "2026-01-30T10:00:00Z"),
            ("t13", "created", null, "Write unit tests",          "u2", "2026-02-01T09:00:00Z"),
            ("t14", "created", null, "Performance optimization",  "u1", "2026-02-03T08:00:00Z"),
            ("t15", "created", null, "App store submission",      "u1", "2026-02-05T09:00:00Z"),

            // Task t16 — created → done
            ("t16", "created",        null,  "Define API spec", "u1", "2026-02-02T09:00:00Z"),
            ("t16", "status_changed", "New", "Done",            "u1", "2026-02-06T14:00:00Z"),

            // Task t17 — created → done
            ("t17", "created",        null,  "Setup .NET project", "u1", "2026-02-02T10:00:00Z"),
            ("t17", "status_changed", "New", "Done",               "u1", "2026-02-04T11:00:00Z"),

            // Task t18 — created → done
            ("t18", "created",        null,  "Configure MongoDB", "u2", "2026-02-03T09:00:00Z"),
            ("t18", "status_changed", "New", "Done",              "u2", "2026-02-07T16:00:00Z"),

            // Task t19 — created → inProgress
            ("t19", "created",        null,  "Implement auth endpoints", "u1", "2026-02-05T09:00:00Z"),
            ("t19", "status_changed", "New", "Active",                   "u1", "2026-02-10T09:00:00Z"),

            // Task t20 — created → inProgress
            ("t20", "created",        null,  "Build CRUD for projects", "u2", "2026-02-06T10:00:00Z"),
            ("t20", "status_changed", "New", "Active",                  "u2", "2026-02-12T10:00:00Z"),

            // Task t21 — created → inProgress
            ("t21", "created",        null,  "Build CRUD for tasks", "u1", "2026-02-07T09:00:00Z"),
            ("t21", "status_changed", "New", "Active",               "u1", "2026-02-13T09:00:00Z"),

            // Tasks t22–t30 — just created
            ("t22", "created", null, "Add rate limiting",          "u1", "2026-02-08T08:00:00Z"),
            ("t23", "created", null, "Setup logging",              "u2", "2026-02-09T09:00:00Z"),
            ("t24", "created", null, "Write integration tests",    "u1", "2026-02-10T10:00:00Z"),
            ("t25", "created", null, "API documentation page",     "u2", "2026-02-11T08:00:00Z"),
            ("t26", "created", null, "Implement webhooks",         "u1", "2026-02-12T09:00:00Z"),
            ("t27", "created", null, "Add caching layer",          "u2", "2026-02-13T10:00:00Z"),
            ("t28", "created", null, "Health check endpoint",      "u1", "2026-02-14T08:00:00Z"),
            ("t29", "created", null, "Versioning strategy",        "u1", "2026-02-15T09:00:00Z"),
            ("t30", "created", null, "Deploy to staging",          "u2", "2026-02-16T10:00:00Z"),
        };

        // For assignee_changed audit logs, oldValue/newValue are user IDs
        var auditLogs = auditData.Select(a => new AuditLog
        {
            Id = ObjectId.GenerateNewId().ToString(),
            TaskId = taskIds[a.taskKey],
            UserId = assignee(a.userKey)!,
            Action = a.action,
            OldValue = a.action == "assignee_changed" ? assignee(a.oldValue!) : a.oldValue,
            NewValue = a.action == "assignee_changed" ? assignee(a.newValue!) : a.newValue,
            CreatedAt = DateTime.Parse(a.timestamp).ToUniversalTime()
        }).ToList();

        await _context.AuditLogs.InsertManyAsync(auditLogs);
    }
}
