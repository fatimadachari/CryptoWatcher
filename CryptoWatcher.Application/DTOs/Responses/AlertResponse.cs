using CryptoWatcher.Domain.Enums;

namespace CryptoWatcher.Application.DTOs.Responses;

public record AlertResponse(
    int Id,
    int UserId,
    string CryptoSymbol,
    decimal TargetPrice,
    PriceCondition Condition,
    AlertStatus Status,
    DateTime CreatedAt,
    DateTime? TriggeredAt
);