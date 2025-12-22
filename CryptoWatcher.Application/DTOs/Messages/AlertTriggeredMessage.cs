namespace CryptoWatcher.Application.DTOs.Messages;

public record AlertTriggeredMessage(
    int AlertId,
    int UserId,
    string UserEmail,
    string CryptoSymbol,
    decimal TargetPrice,
    decimal CurrentPrice,
    string Condition,
    DateTime TriggeredAt
);