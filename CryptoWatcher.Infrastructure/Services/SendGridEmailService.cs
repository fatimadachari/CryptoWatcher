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

    public SendGridEmailService(IConfiguration configuration, ILogger<SendGridEmailService> logger)
    {
        _apiKey = configuration["SendGrid:ApiKey"] ?? throw new InvalidOperationException("SendGrid API Key não configurada");
        _fromEmail = configuration["SendGrid:FromEmail"] ?? throw new InvalidOperationException("SendGrid FromEmail não configurado");
        _fromName = configuration["SendGrid:FromName"] ?? "CryptoWatcher";
        _logger = logger;
    }

    public async Task SendAlertNotificationAsync(string toEmail, string userName, string cryptoSymbol, decimal targetPrice, decimal currentPrice, string condition, CancellationToken cancellationToken = default)
    {
        var client = new SendGridClient(_apiKey);
        var from = new EmailAddress(_fromEmail, _fromName);
        var to = new EmailAddress(toEmail, userName);
        var subject = $"🔔 Alerta CryptoWatcher: {cryptoSymbol} {condition} ${targetPrice:N2}";

        var htmlContent = GenerateEmailTemplate(userName, cryptoSymbol, targetPrice, currentPrice, condition);

        var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);

        try
        {
            var response = await client.SendEmailAsync(msg, cancellationToken);
            _logger.LogInformation($"📧 Email enviado com sucesso para: {toEmail}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"❌ Erro ao enviar email para: {toEmail}");
            throw;
        }
    }

    private string GenerateEmailTemplate(string userName, string cryptoSymbol, decimal targetPrice, decimal currentPrice, string condition)
    {
        var priceChangePercent = ((currentPrice - targetPrice) / targetPrice * 100);
        var isAbove = condition.ToLower().Contains("acima") || condition.ToLower().Contains("above");
        var conditionText = isAbove ? "ACIMA" : "ABAIXO";
        var conditionIcon = isAbove ? "📈" : "📉";
        var conditionColor = isAbove ? "#00ff88" : "#ff4444";

        return $@"
<!DOCTYPE html>
<html lang='pt-BR'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Alerta CryptoWatcher</title>
</head>
<body style='margin: 0; padding: 0; background: linear-gradient(135deg, #0a0e0d 0%, #0d1410 50%, #0a0e0d 100%); font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, ""Helvetica Neue"", Arial, sans-serif;'>
    <table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%' style='background: linear-gradient(135deg, #0a0e0d 0%, #0d1410 50%, #0a0e0d 100%); padding: 40px 20px;'>
        <tr>
            <td align='center'>
                <!-- Container Principal -->
                <table role='presentation' cellspacing='0' cellpadding='0' border='0' width='600' style='max-width: 600px; background: rgba(15, 25, 20, 0.8); border: 1px solid rgba(0, 255, 136, 0.2); border-radius: 20px; overflow: hidden; box-shadow: 0 20px 60px rgba(0, 255, 136, 0.15);'>
                    
                    <!-- Header com Logo -->
                    <tr>
                        <td style='background: linear-gradient(135deg, rgba(0, 255, 136, 0.1) 0%, rgba(0, 204, 106, 0.05) 100%); padding: 40px 40px 30px; text-align: center; border-bottom: 1px solid rgba(0, 255, 136, 0.2);'>
                            <div style='display: inline-block; background: rgba(0, 255, 136, 0.1); padding: 15px; border-radius: 16px; border: 1px solid rgba(0, 255, 136, 0.3); margin-bottom: 20px;'>
                                <span style='font-size: 32px;'>📊</span>
                            </div>
                           <h1 style='margin: 0; font-size: 28px; font-weight: 700; color: #ffffff;'>
    Crypto<span style='color: #00ff88;'>Watcher</span>
</h1>
                            <p style='margin: 8px 0 0; color: #9ca3af; font-size: 14px; font-weight: 500;'>
                                Monitoramento em Tempo Real
                            </p>
                        </td>
                    </tr>

                    <!-- Saudação -->
                    <tr>
                        <td style='padding: 30px 40px 20px;'>
                            <p style='margin: 0; font-size: 16px; color: #e5e7eb; font-weight: 500;'>
                                Olá, <span style='color: #00ff88; font-weight: 700;'>{userName}</span>! 👋
                            </p>
                        </td>
                    </tr>

                    <!-- Alerta Principal -->
                    <tr>
                        <td style='padding: 0 40px 30px;'>
                            <table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%' style='background: rgba(10, 14, 13, 0.6); border: 2px solid {conditionColor}; border-radius: 16px; padding: 30px; box-shadow: 0 0 30px {conditionColor}40;'>
                                <tr>
                                    <td align='center'>
                                        <div style='font-size: 48px; margin-bottom: 15px;'>{conditionIcon}</div>
                                        <h2 style='margin: 0 0 10px; font-size: 24px; color: {conditionColor}; font-weight: 700; text-transform: uppercase; letter-spacing: 1px;'>
                                            ALERTA DISPARADO!
                                        </h2>
                                        <p style='margin: 0; font-size: 16px; color: #9ca3af;'>
                                            Seu alerta foi atingido
                                        </p>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                    <!-- Detalhes da Criptomoeda -->
                    <tr>
                        <td style='padding: 0 40px 30px;'>
                            <table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'>
                                <tr>
                                    <td style='background: rgba(10, 14, 13, 0.4); border: 1px solid rgba(75, 85, 99, 0.4); border-radius: 12px; padding: 20px; margin-bottom: 15px;'>
                                        <p style='margin: 0 0 5px; font-size: 12px; color: #9ca3af; font-weight: 600; text-transform: uppercase; letter-spacing: 0.5px;'>
                                            Criptomoeda
                                        </p>
                                        <p style='margin: 0; font-size: 24px; color: #ffffff; font-weight: 700;'>
                                            {cryptoSymbol}
                                        </p>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                    <!-- Grid de Informações -->
                    <tr>
                        <td style='padding: 0 40px 30px;'>
                            <table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'>
                                <tr>
                                    <td width='48%' style='background: rgba(10, 14, 13, 0.4); border: 1px solid rgba(75, 85, 99, 0.4); border-radius: 12px; padding: 20px; vertical-align: top;'>
                                        <p style='margin: 0 0 8px; font-size: 12px; color: #9ca3af; font-weight: 600; text-transform: uppercase; letter-spacing: 0.5px;'>
                                            Preço Alvo
                                        </p>
                                        <p style='margin: 0; font-size: 22px; color: #ffffff; font-weight: 700;'>
                                            ${targetPrice:N2}
                                        </p>
                                    </td>
                                    <td width='4%'></td>
                                    <td width='48%' style='background: rgba(10, 14, 13, 0.4); border: 1px solid rgba(75, 85, 99, 0.4); border-radius: 12px; padding: 20px; vertical-align: top;'>
                                        <p style='margin: 0 0 8px; font-size: 12px; color: #9ca3af; font-weight: 600; text-transform: uppercase; letter-spacing: 0.5px;'>
                                            Preço Atual
                                        </p>
                                        <p style='margin: 0; font-size: 22px; color: {conditionColor}; font-weight: 700;'>
                                            ${currentPrice:N2}
                                        </p>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                    <!-- Condição e Variação -->
                    <tr>
                        <td style='padding: 0 40px 30px;'>
                            <table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'>
                                <tr>
                                    <td width='48%' style='background: rgba(10, 14, 13, 0.4); border: 1px solid rgba(75, 85, 99, 0.4); border-radius: 12px; padding: 20px; vertical-align: top;'>
                                        <p style='margin: 0 0 8px; font-size: 12px; color: #9ca3af; font-weight: 600; text-transform: uppercase; letter-spacing: 0.5px;'>
                                            Condição
                                        </p>
                                        <p style='margin: 0; font-size: 18px; color: {conditionColor}; font-weight: 700;'>
                                            {conditionIcon} {conditionText}
                                        </p>
                                    </td>
                                    <td width='4%'></td>
                                    <td width='48%' style='background: rgba(10, 14, 13, 0.4); border: 1px solid rgba(75, 85, 99, 0.4); border-radius: 12px; padding: 20px; vertical-align: top;'>
                                        <p style='margin: 0 0 8px; font-size: 12px; color: #9ca3af; font-weight: 600; text-transform: uppercase; letter-spacing: 0.5px;'>
                                            Variação
                                        </p>
                                        <p style='margin: 0; font-size: 18px; color: {conditionColor}; font-weight: 700;'>
                                            {(priceChangePercent >= 0 ? "+" : "")}{priceChangePercent:N2}%
                                        </p>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                    <!-- Botão CTA -->
                    <tr>
                        <td style='padding: 10px 40px 40px;' align='center'>
                            <a href='http://localhost:5173/dashboard' style='display: inline-block; background: linear-gradient(135deg, #00ff88 0%, #00cc6a 100%); color: #000000; text-decoration: none; padding: 16px 40px; border-radius: 12px; font-weight: 700; font-size: 16px; box-shadow: 0 10px 30px rgba(0, 255, 136, 0.3); transition: all 0.3s ease;'>
                                Ver Todos os Alertas →
                            </a>
                        </td>
                    </tr>

                    <!-- Footer -->
                    <tr>
                        <td style='background: rgba(10, 14, 13, 0.6); padding: 30px 40px; text-align: center; border-top: 1px solid rgba(0, 255, 136, 0.2);'>
                            <p style='margin: 0 0 10px; font-size: 14px; color: #6b7280; line-height: 1.6;'>
                                Você está recebendo este email porque configurou um alerta no CryptoWatcher.
                            </p>
                            <p style='margin: 0; font-size: 12px; color: #4b5563;'>
                                © 2026 CryptoWatcher. Todos os direitos reservados.
                            </p>
                            <div style='margin-top: 20px; padding-top: 20px; border-top: 1px solid rgba(75, 85, 99, 0.3);'>
                                <p style='margin: 0; font-size: 11px; color: #4b5563;'>
                                    Este é um email automático. Por favor, não responda.
                                </p>
                            </div>
                        </td>
                    </tr>

                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }
}