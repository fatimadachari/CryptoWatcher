using CryptoWatcher.Application.DTOs.Messages;
using CryptoWatcher.Application.Interfaces.Repositories;
using CryptoWatcher.Application.Interfaces.Services;
using CryptoWatcher.Domain.Entities;

namespace CryptoWatcher.Worker.Services;

public class PriceMonitorService
{
    private readonly IAlertRepository _alertRepository;
    private readonly IPriceService _priceService;
    private readonly IMessagePublisher _messagePublisher;
    private readonly ILogger<PriceMonitorService> _logger;

    public PriceMonitorService(
        IAlertRepository alertRepository,
        IPriceService priceService,
        IMessagePublisher messagePublisher,
        ILogger<PriceMonitorService> logger)
    {
        _alertRepository = alertRepository;
        _priceService = priceService;
        _messagePublisher = messagePublisher;
        _logger = logger;
    }

    public async Task MonitorPricesAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("?? Iniciando monitoramento de preços...");

        var activeAlerts = await _alertRepository.GetActiveAlertsAsync(cancellationToken);
        var alertsList = activeAlerts.ToList();

        if (!alertsList.Any())
        {
            _logger.LogInformation("Nenhum alerta ativo encontrado.");
            return;
        }

        _logger.LogInformation("Encontrados {Count} alertas ativos", alertsList.Count);

        var alertsBySymbol = alertsList.GroupBy(a => a.CryptoSymbol);

        foreach (var group in alertsBySymbol)
        {
            var symbol = group.Key;

            try
            {
                var currentPrice = await _priceService.GetCurrentPriceAsync(symbol, cancellationToken);

                if (currentPrice is null)
                {
                    _logger.LogWarning("Não foi possível obter o preço de {Symbol}", symbol);
                    continue;
                }

                _logger.LogInformation("{Symbol}: ${Price:N2}", symbol, currentPrice.Value);

                foreach (var alert in group)
                {
                    if (alert.ShouldTrigger(currentPrice.Value))
                    {
                        _logger.LogWarning(
                            "?? ALERTA DISPARADO! User: {UserId}, {Symbol} {Condition} ${Target} (Atual: ${Current})",
                            alert.UserId,
                            alert.CryptoSymbol,
                            alert.Condition,
                            alert.TargetPrice,
                            currentPrice.Value
                        );

                        // Marcar como disparado
                        alert.MarkAsTriggered();
                        await _alertRepository.UpdateAsync(alert, cancellationToken);

                        // Publicar mensagem na fila
                        var message = new AlertTriggeredMessage(
                            alert.Id,
                            alert.UserId,
                            alert.User.Email,
                            alert.CryptoSymbol,
                            alert.TargetPrice,
                            currentPrice.Value,
                            alert.Condition.ToString(),
                            DateTime.UtcNow
                        );

                        await _messagePublisher.PublishAlertTriggeredAsync(message, cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar alertas de {Symbol}", symbol);
            }
        }

        _logger.LogInformation("? Monitoramento concluído.");
    }
}