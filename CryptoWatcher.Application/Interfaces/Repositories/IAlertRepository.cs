using CryptoWatcher.Domain.Entities;
using CryptoWatcher.Domain.Enums;

namespace CryptoWatcher.Application.Interfaces.Repositories;

public interface IAlertRepository
{
    Task<CryptoAlert?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<CryptoAlert>> GetActiveAlertsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<CryptoAlert>> GetAlertsByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<CryptoAlert> CreateAsync(CryptoAlert alert, CancellationToken cancellationToken = default);
    Task UpdateAsync(CryptoAlert alert, CancellationToken cancellationToken = default);
}