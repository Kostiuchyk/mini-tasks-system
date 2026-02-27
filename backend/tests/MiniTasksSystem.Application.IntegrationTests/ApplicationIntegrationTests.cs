using Microsoft.Extensions.DependencyInjection;
using MiniTasksSystem.Application.Comments;
using MiniTasksSystem.Application.Projects;
using MiniTasksSystem.Application.Tasks;
using Xunit;

namespace MiniTasksSystem.Application.IntegrationTests;

public class ApplicationIntegrationTests : IClassFixture<WebAppFactory>
{
    private const string TestUserId = "test-user-id";

    private readonly WebAppFactory _factory;

    public ApplicationIntegrationTests(WebAppFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ProjectService_Create_PersistsAndReturnsProject()
    {
        using var scope = _factory.Services.CreateScope();
        var projects = scope.ServiceProvider.GetRequiredService<IProjectService>();

        var result = await projects.Create(new ProjectDto(string.Empty, "My Project", "Some description", default));

        Assert.NotEmpty(result.Id);
        Assert.Equal("My Project", result.Name);
        Assert.Equal("Some description", result.Description);
    }

    [Fact]
    public async Task ProjectService_Update_ChangesProjectName()
    {
        using var scope = _factory.Services.CreateScope();
        var projects = scope.ServiceProvider.GetRequiredService<IProjectService>();

        var created = await projects.Create(new ProjectDto(string.Empty, "Original Name", null, default));
        await projects.Update(created.Id, new ProjectDto(string.Empty, "Updated Name", null, default));

        var fetched = await projects.GetById(created.Id);

        Assert.Equal("Updated Name", fetched.Name);
    }

    [Fact]
    public async Task TaskService_Create_PersistsAndReturnsTask()
    {
        using var scope = _factory.Services.CreateScope();
        var projects = scope.ServiceProvider.GetRequiredService<IProjectService>();
        var tasks = scope.ServiceProvider.GetRequiredService<ITaskService>();

        var project = await projects.Create(new ProjectDto(string.Empty, "Task Project", null, default));

        var dto = new TaskItemDto(string.Empty, 0, "Fix Bug", "Reproducible crash", "New", "High", project.Id, null, default, default);
        var result = await tasks.Create(dto, TestUserId);

        Assert.NotEmpty(result.Id);
        Assert.Equal("Fix Bug", result.Title);
        Assert.Equal("High", result.Priority);
        Assert.Equal("New", result.Status);
        Assert.Equal(project.Id, result.ProjectId);
    }

    [Fact]
    public async Task TaskService_GetTasks_FiltersByProjectId()
    {
        using var scope = _factory.Services.CreateScope();
        var projects = scope.ServiceProvider.GetRequiredService<IProjectService>();
        var tasks = scope.ServiceProvider.GetRequiredService<ITaskService>();

        var projectA = await projects.Create(new ProjectDto(string.Empty, "Project A", null, default));
        var projectB = await projects.Create(new ProjectDto(string.Empty, "Project B", null, default));

        await tasks.Create(new TaskItemDto(string.Empty, 0, "Task in A", null, "New", "Low", projectA.Id, null, default, default), TestUserId);
        await tasks.Create(new TaskItemDto(string.Empty, 0, "Another in A", null, "New", "Low", projectA.Id, null, default, default), TestUserId);
        await tasks.Create(new TaskItemDto(string.Empty, 0, "Task in B", null, "New", "Low", projectB.Id, null, default, default), TestUserId);

        var result = await tasks.GetTasks(new TaskQueryParams(ProjectId: projectA.Id));

        Assert.All(result.Items, t => Assert.Equal(projectA.Id, t.ProjectId));
        Assert.Equal(2, result.TotalCount);
    }

    [Fact]
    public async Task CommentService_Create_AppearsInTaskComments()
    {
        using var scope = _factory.Services.CreateScope();
        var projects = scope.ServiceProvider.GetRequiredService<IProjectService>();
        var tasks = scope.ServiceProvider.GetRequiredService<ITaskService>();
        var comments = scope.ServiceProvider.GetRequiredService<ICommentService>();

        var project = await projects.Create(new ProjectDto(string.Empty, "Comment Project", null, default));
        var task = await tasks.Create(new TaskItemDto(string.Empty, 0, "Commented Task", null, "New", "Medium", project.Id, null, default, default), TestUserId);

        var comment = await comments.Create(new CommentDto(string.Empty, task.Id, TestUserId, "Looks good!", default));

        var taskComments = await comments.GetByTaskId(task.Id);

        Assert.Single(taskComments);
        Assert.Equal("Looks good!", taskComments[0].Text);
        Assert.Equal(task.Id, taskComments[0].TaskId);
        Assert.Equal(comment.Id, taskComments[0].Id);
    }
}
