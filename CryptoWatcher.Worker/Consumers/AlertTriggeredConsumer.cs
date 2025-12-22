using CryptoWatcher.Application.DTOs.Messages;
using MassTransit;

namespace CryptoWatcher.Worker.Consumers;

public class AlertTriggeredConsumer : IConsumer<AlertTriggeredMessage>
{
    private readonly ILogger<AlertTriggeredConsumer> _logger;

    public AlertTriggeredConsumer(ILogger<AlertTriggeredConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<AlertTriggeredMessage> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "📩 Mensagem recebida da fila: Alert {AlertId}",
            message.AlertId
        );

        try
        {
            // Simular processamento (envio de notificação)
            await SendNotificationAsync(message);

            _logger.LogInformation(
                "✅ Notificação enviada com sucesso para {Email}",
                message.UserEmail
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "❌ Erro ao processar alerta {AlertId}",
                message.AlertId
            );

            // Se lançar exceção, MassTransit faz NACK e reprocessa
            throw;
        }
    }

    private async Task SendNotificationAsync(AlertTriggeredMessage message)
    {
        // Simular delay de envio de email/SMS
        await Task.Delay(500);

        _logger.LogWarning(
            """
            
            ═══════════════════════════════════════════════════════
            🔔 NOTIFICAÇÃO DE ALERTA
            ═══════════════════════════════════════════════════════
            Para: {Email}
            Assunto: Alerta de {Symbol} Disparado!
            
            Olá,
            
            Seu alerta foi disparado:
            • Criptomoeda: {Symbol}
            • Condição: {Condition}
            • Preço Alvo: ${Target:N2}
            • Preço Atual: ${Current:N2}
            • Data: {Date}
            
            Acesse o sistema para mais detalhes.
            ═══════════════════════════════════════════════════════
            """,
            message.UserEmail,
            message.CryptoSymbol,
            message.CryptoSymbol,
            message.Condition,
            message.TargetPrice,
            message.CurrentPrice,
            message.TriggeredAt
        );
    }
}