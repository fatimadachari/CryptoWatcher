using CryptoWatcher.Domain.Enums;

namespace CryptoWatcher.Application.DTOs.Requests;

public record CreateAlertRequest(
    int UserId,
    string CryptoSymbol,
    decimal TargetPrice,
    PriceCondition Condition
);