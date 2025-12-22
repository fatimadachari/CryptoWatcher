using CryptoWatcher.Application.DTOs.Messages;

namespace CryptoWatcher.Application.Interfaces.Services;

public interface IMessagePublisher
{
    Task PublishAlertTriggeredAsync(
        AlertTriggeredMessage message,
        CancellationToken cancellationToken = default
    );
}