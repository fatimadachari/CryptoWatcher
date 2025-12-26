using CryptoWatcher.Application.Interfaces.Repositories;
using CryptoWatcher.Domain.Entities;

namespace CryptoWatcher.Application.UseCases.Alerts;

public class GetActiveAlertsUseCase
{
    private readonly IAlertRepository _alertRepository;

    public GetActiveAlertsUseCase(IAlertRepository alertRepository)
    {
        _alertRepository = alertRepository;
    }

    // ⬅️ AGORA RECEBE USERID
    public async Task<IEnumerable<CryptoAlert>> ExecuteAsync(int userId, CancellationToken cancellationToken = default)
    {
        // ⬅️ BUSCA APENAS OS ALERTAS DO USUÁRIO
        var alerts = await _alertRepository.GetActiveAlertsByUserAsync(userId, cancellationToken);
        return alerts;
    }
}