namespace CryptoWatcher.Application.Interfaces.Services;

public interface IEmailService
{
    Task SendAlertNotificationAsync(
        string toEmail,
        string userName,
        string cryptoSymbol,
        decimal targetPrice,
        decimal currentPrice,
        string condition,
        CancellationToken cancellationToken = default);
}