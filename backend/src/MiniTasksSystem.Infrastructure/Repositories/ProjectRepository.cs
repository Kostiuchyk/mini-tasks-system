using MongoDB.Driver;
using MiniTasksSystem.Application.Projects;
using MiniTasksSystem.Domain.Projects;
using MiniTasksSystem.Infrastructure.Persistence;

namespace MiniTasksSystem.Infrastructure.Repositories;

internal sealed class ProjectRepository(MongoDbContext context, MongoSessionAccessor sessionAccessor) : IProjectRepository
{
    private readonly MongoDbContext _context = context;
    private readonly MongoSessionAccessor _sessionAccessor = sessionAccessor;
    private IClientSessionHandle? Session => _sessionAccessor.Session;

    public async Task<Project?> GetById(string id)
    {
        if (Session is { } s)
        {
            return await _context.Projects.Find(s, p => p.Id == id).FirstOrDefaultAsync();
        }

        return await _context.Projects.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Project>> GetAll()
    {
        if (Session is { } s)
        {
            return await _context.Projects.Find(s, _ => true).SortByDescending(p => p.CreatedAt).ToListAsync();
        }

        return await _context.Projects.Find(_ => true).SortByDescending(p => p.CreatedAt).ToListAsync();
    }

    public async Task Create(Project project)
    {
        if (Session is { } s)
        {
            await _context.Projects.InsertOneAsync(s, project);
            return;
        }

        await _context.Projects.InsertOneAsync(project);
    }

    public async Task Update(Project project)
    {
        if (Session is { } s)
        {
            await _context.Projects.ReplaceOneAsync(s, p => p.Id == project.Id, project);
            return;
        }

        await _context.Projects.ReplaceOneAsync(p => p.Id == project.Id, project);
    }

    public async Task Delete(string id)
    {
        if (Session is { } s)
        {
            await _context.Projects.DeleteOneAsync(s, p => p.Id == id);
            return;
        }

        await _context.Projects.DeleteOneAsync(p => p.Id == id);
    }
}
