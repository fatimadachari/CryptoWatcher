using CryptoWatcher.Application.DTOs.Messages;
using CryptoWatcher.Application.Interfaces.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CryptoWatcher.Infrastructure.Services;

public class RabbitMqPublisher : IMessagePublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<RabbitMqPublisher> _logger;

    public RabbitMqPublisher(IPublishEndpoint publishEndpoint, ILogger<RabbitMqPublisher> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task PublishAlertTriggeredAsync(
        AlertTriggeredMessage message,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _publishEndpoint.Publish(message, cancellationToken);

            _logger.LogInformation(
                "📤 Mensagem publicada: Alert {AlertId} - {Symbol} ${Price}",
                message.AlertId,
                message.CryptoSymbol,
                message.CurrentPrice
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao publicar mensagem do Alert {AlertId}", message.AlertId);
            throw;
        }
    }
}