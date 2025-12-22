using CryptoWatcher.Application.DTOs.Responses;
using CryptoWatcher.Application.Interfaces.Repositories;

namespace CryptoWatcher.Application.UseCases.Alerts;

public class GetActiveAlertsUseCase
{
    private readonly IAlertRepository _alertRepository;

    public GetActiveAlertsUseCase(IAlertRepository alertRepository)
    {
        _alertRepository = alertRepository;
    }

    public async Task<IEnumerable<AlertResponse>> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        var alerts = await _alertRepository.GetActiveAlertsAsync(cancellationToken);

        return alerts.Select(alert => new AlertResponse(
            alert.Id,
            alert.UserId,
            alert.CryptoSymbol,
            alert.TargetPrice,
            alert.Condition,
            alert.Status,
            alert.CreatedAt,
            alert.TriggeredAt
        ));
    }
}