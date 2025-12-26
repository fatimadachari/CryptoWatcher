using CryptoWatcher.Domain.Entities;

namespace CryptoWatcher.Application.Interfaces.Repositories;

public interface IAlertRepository
{
    Task<CryptoAlert?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<CryptoAlert>> GetActiveAlertsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<CryptoAlert>> GetActiveAlertsByUserAsync(int userId, CancellationToken cancellationToken = default); // ⬅️ NOVO
    Task CreateAsync(CryptoAlert alert, CancellationToken cancellationToken = default);
    Task UpdateAsync(CryptoAlert alert, CancellationToken cancellationToken = default);
}