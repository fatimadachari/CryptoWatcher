using CryptoWatcher.Application.Interfaces.Repositories;
using CryptoWatcher.Application.Interfaces.Services;
using CryptoWatcher.Domain.Entities;

namespace CryptoWatcher.Worker.Services;

public class PriceMonitorService
{
    private readonly IAlertRepository _alertRepository;
    private readonly IPriceService _priceService;
    private readonly ILogger<PriceMonitorService> _logger;

    public PriceMonitorService(
        IAlertRepository alertRepository,
        IPriceService priceService,
        ILogger<PriceMonitorService> logger)
    {
        _alertRepository = alertRepository;
        _priceService = priceService;
        _logger = logger;
    }

    public async Task MonitorPricesAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("?? Iniciando monitoramento de preços...");

        // 1. Buscar todos os alertas ativos
        var activeAlerts = await _alertRepository.GetActiveAlertsAsync(cancellationToken);
        var alertsList = activeAlerts.ToList();

        if (!alertsList.Any())
        {
            _logger.LogInformation("Nenhum alerta ativo encontrado.");
            return;
        }

        _logger.LogInformation("Encontrados {Count} alertas ativos", alertsList.Count);

        // 2. Agrupar alertas por símbolo para otimizar chamadas à API
        var alertsBySymbol = alertsList.GroupBy(a => a.CryptoSymbol);

        foreach (var group in alertsBySymbol)
        {
            var symbol = group.Key;

            try
            {
                // 3. Consultar preço atual
                var currentPrice = await _priceService.GetCurrentPriceAsync(symbol, cancellationToken);

                if (currentPrice is null)
                {
                    _logger.LogWarning("Não foi possível obter o preço de {Symbol}", symbol);
                    continue;
                }

                _logger.LogInformation("{Symbol}: ${Price:N2}", symbol, currentPrice.Value);

                // 4. Verificar cada alerta desse símbolo
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

                        // 5. Marcar como disparado
                        alert.MarkAsTriggered();
                        await _alertRepository.UpdateAsync(alert, cancellationToken);

                        // TODO: Publicar mensagem no RabbitMQ (próxima etapa)
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