using Microsoft.Extensions.DependencyInjection;

namespace DapperSample.Infrastructure;

public class UnitOfWorkManager:IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly AsyncLocal<IUnitOfWork> _currentUnitOfWork = new();

    public UnitOfWorkManager(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IUnitOfWork Begin()
    {
        if (_currentUnitOfWork.Value != null)
        {
            throw new InvalidOperationException("A unit of work is already active in this scope.");
        }
        
        _currentUnitOfWork.Value = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IUnitOfWork>();
        return _currentUnitOfWork.Value;
    }

    public async Task CommitAsync()
    {
        if (_currentUnitOfWork.Value == null)
        {
            throw new InvalidOperationException("No active unit of work.");
        }

        await _currentUnitOfWork.Value.CommitAsync();
    }

    public void Dispose()
    {
        _currentUnitOfWork.Value?.Dispose();
        _currentUnitOfWork.Value = null;
    }
}