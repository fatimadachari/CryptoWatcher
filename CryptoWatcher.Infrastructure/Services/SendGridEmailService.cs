using CryptoWatcher.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace CryptoWatcher.Infrastructure.Services;

public class SendGridEmailService : IEmailService
{
    private readonly string _apiKey;
    private readonly string _fromEmail;
    private readonly string _fromName;
    private readonly ILogger<SendGridEmailService> _logger;

    public SendGridEmailService(
        IConfiguration configuration,
        ILogger<SendGridEmailService> logger)
    {
        _apiKey = configuration["SendGrid:ApiKey"]
            ?? throw new InvalidOperationException("SendGrid API Key não configurada");
        _fromEmail = configuration["SendGrid:FromEmail"] ?? "noreply@cryptowatcher.com";
        _fromName = configuration["SendGrid:FromName"] ?? "CryptoWatcher";
        _logger = logger;
    }

    public async Task SendAlertNotificationAsync(
        string toEmail,
        string userName,
        string cryptoSymbol,
        decimal targetPrice,
        decimal currentPrice,
        string condition,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress(_fromEmail, _fromName);
            var to = new EmailAddress(toEmail, userName);
            var subject = $"🚨 Alerta de {cryptoSymbol} Disparado!";

            var htmlContent = BuildEmailHtml(
                userName,
                cryptoSymbol,
                targetPrice,
                currentPrice,
                condition
            );

            var msg = MailHelper.CreateSingleEmail(
                from,
                to,
                subject,
                plainTextContent: BuildEmailPlainText(userName, cryptoSymbol, targetPrice, currentPrice, condition),
                htmlContent
            );

            var response = await client.SendEmailAsync(msg, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation(
                    "✅ Email enviado com sucesso para {Email} - {Symbol} {Condition} ${Price}",
                    toEmail, cryptoSymbol, condition, currentPrice
                );
            }
            else
            {
                var body = await response.Body.ReadAsStringAsync(cancellationToken);
                _logger.LogError(
                    "❌ Erro ao enviar email. Status: {Status}, Body: {Body}",
                    response.StatusCode, body
                );
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Exceção ao enviar email para {Email}", toEmail);
            throw;
        }
    }

    private static string BuildEmailHtml(
        string userName,
        string cryptoSymbol,
        decimal targetPrice,
        decimal currentPrice,
        string condition)
    {
        var emoji = condition == "Above" ? "📈" : "📉";
        var conditionText = condition == "Above" ? "acima de" : "abaixo de";
        var color = condition == "Above" ? "#10b981" : "#ef4444";

        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
</head>
<body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f3f4f6;'>
    <table width='100%' cellpadding='0' cellspacing='0' style='background-color: #f3f4f6; padding: 20px;'>
        <tr>
            <td align='center'>
                <table width='600' cellpadding='0' cellspacing='0' style='background-color: #ffffff; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);'>
                    <!-- Header -->
                    <tr>
                        <td style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 30px; text-align: center; border-radius: 8px 8px 0 0;'>
                            <h1 style='color: #ffffff; margin: 0; font-size: 28px;'>{emoji} Alerta Disparado!</h1>
                        </td>
                    </tr>
                    
                    <!-- Body -->
                    <tr>
                        <td style='padding: 40px 30px;'>
                            <p style='font-size: 16px; color: #374151; margin: 0 0 20px 0;'>
                                Olá <strong>{userName}</strong>,
                            </p>
                            
                            <p style='font-size: 16px; color: #374151; margin: 0 0 30px 0;'>
                                Seu alerta de <strong>{cryptoSymbol}</strong> foi disparado!
                            </p>
                            
                            <!-- Alert Box -->
                            <table width='100%' cellpadding='0' cellspacing='0' style='background-color: #f9fafb; border-left: 4px solid {color}; border-radius: 4px; margin: 0 0 30px 0;'>
                                <tr>
                                    <td style='padding: 20px;'>
                                        <table width='100%' cellpadding='0' cellspacing='0'>
                                            <tr>
                                                <td style='padding: 10px 0;'>
                                                    <span style='color: #6b7280; font-size: 14px;'>Criptomoeda:</span><br>
                                                    <strong style='color: #111827; font-size: 18px;'>{cryptoSymbol}</strong>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style='padding: 10px 0;'>
                                                    <span style='color: #6b7280; font-size: 14px;'>Condição:</span><br>
                                                    <strong style='color: #111827; font-size: 18px;'>Preço {conditionText}</strong>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style='padding: 10px 0;'>
                                                    <span style='color: #6b7280; font-size: 14px;'>Preço Alvo:</span><br>
                                                    <strong style='color: #111827; font-size: 18px;'>${targetPrice:N2}</strong>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style='padding: 10px 0;'>
                                                    <span style='color: #6b7280; font-size: 14px;'>Preço Atual:</span><br>
                                                    <strong style='color: {color}; font-size: 24px;'>${currentPrice:N2}</strong>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            
                            <p style='font-size: 14px; color: #6b7280; margin: 0;'>
                                Este é um alerta automático do CryptoWatcher. O alerta foi marcado como disparado e não enviará mais notificações.
                            </p>
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style='background-color: #f9fafb; padding: 20px; text-align: center; border-radius: 0 0 8px 8px;'>
                            <p style='font-size: 12px; color: #9ca3af; margin: 0;'>
                                © 2025 CryptoWatcher. Todos os direitos reservados.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }

    private static string BuildEmailPlainText(
        string userName,
        string cryptoSymbol,
        decimal targetPrice,
        decimal currentPrice,
        string condition)
    {
        var conditionText = condition == "Above" ? "acima de" : "abaixo de";

        return $@"
Olá {userName},

Seu alerta de {cryptoSymbol} foi disparado!

Criptomoeda: {cryptoSymbol}
Condição: Preço {conditionText}
Preço Alvo: ${targetPrice:N2}
Preço Atual: ${currentPrice:N2}

Este é um alerta automático do CryptoWatcher.

© 2024 CryptoWatcher
";
    }
}