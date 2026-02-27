using MongoDB.Driver;
using MiniTasksSystem.Application.Common;

namespace MiniTasksSystem.Infrastructure.Persistence;

internal sealed class MongoUnitOfWork(MongoDbContext context, MongoSessionAccessor sessionAccessor) : IUnitOfWork
{
    private readonly MongoDbContext _context = context;
    private readonly MongoSessionAccessor _sessionAccessor = sessionAccessor;
    private IClientSessionHandle? _session;

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_sessionAccessor.Session is not null)
        {
            throw new InvalidOperationException("A transaction is already in progress.");
        }

        _session = await _context.Client.StartSessionAsync(cancellationToken: cancellationToken);
        _session.StartTransaction();
        _sessionAccessor.Session = _session;
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_session is null)
        {
            throw new InvalidOperationException("No transaction in progress.");
        }

        try
        {
            await _session.CommitTransactionAsync(cancellationToken);
        }
        finally
        {
            _sessionAccessor.Session = null;
            _session.Dispose();
            _session = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_session is null)
        {
            throw new InvalidOperationException("No transaction in progress.");
        }

        try
        {
            await _session.AbortTransactionAsync(cancellationToken);
        }
        finally
        {
            _sessionAccessor.Session = null;
            _session.Dispose();
            _session = null;
        }
    }
}
