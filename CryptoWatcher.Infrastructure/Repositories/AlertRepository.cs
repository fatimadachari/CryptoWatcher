using CryptoWatcher.Application.Interfaces.Repositories;
using CryptoWatcher.Domain.Entities;
using CryptoWatcher.Domain.Enums;
using CryptoWatcher.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CryptoWatcher.Infrastructure.Repositories;

public class AlertRepository : IAlertRepository
{
    private readonly AppDbContext _context;

    public AlertRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CryptoAlert?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.CryptoAlerts
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<CryptoAlert>> GetActiveAlertsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.CryptoAlerts
            .Include(a => a.User)
            .Where(a => a.Status == AlertStatus.Active)
            .ToListAsync(cancellationToken);
    }

    // ⬅️ NOVO MÉTODO
    public async Task<IEnumerable<CryptoAlert>> GetActiveAlertsByUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.CryptoAlerts
            .Include(a => a.User)
            .Where(a => a.UserId == userId && a.Status == AlertStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task CreateAsync(CryptoAlert alert, CancellationToken cancellationToken = default)
    {
        await _context.CryptoAlerts.AddAsync(alert, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(CryptoAlert alert, CancellationToken cancellationToken = default)
    {
        _context.CryptoAlerts.Update(alert);
        await _context.SaveChangesAsync(cancellationToken);
    }
}