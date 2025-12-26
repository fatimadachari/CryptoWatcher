using CryptoWatcher.Application.DTOs.Messages;
using CryptoWatcher.Application.Interfaces.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CryptoWatcher.Worker.Consumers;

public class AlertTriggeredConsumer : IConsumer<AlertTriggeredMessage>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<AlertTriggeredConsumer> _logger;

    public AlertTriggeredConsumer(
        IEmailService emailService,
        ILogger<AlertTriggeredConsumer> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<AlertTriggeredMessage> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "📩 Mensagem recebida da fila: Alert {AlertId} - {Symbol} ${Price}",
            message.AlertId, message.CryptoSymbol, message.CurrentPrice
        );

        try
        {
            _logger.LogInformation("🔔 Enviando notificação por email...");

            await _emailService.SendAlertNotificationAsync(
                toEmail: message.UserEmail,
                userName: message.UserEmail.Split('@')[0], // Nome temporário do email
                cryptoSymbol: message.CryptoSymbol,
                targetPrice: message.TargetPrice,
                currentPrice: message.CurrentPrice,
                condition: message.Condition.ToString(),
                cancellationToken: context.CancellationToken
            );

            _logger.LogInformation(
                "✅ Email enviado com sucesso para {Email}",
                message.UserEmail
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "❌ Erro ao processar alerta {AlertId} para {Email}",
                message.AlertId,
                message.UserEmail
            );
            throw; // Retry automático via MassTransit
        }
    }
}