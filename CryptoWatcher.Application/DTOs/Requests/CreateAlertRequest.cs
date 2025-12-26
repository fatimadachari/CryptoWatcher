using CryptoWatcher.Domain.Enums;

namespace CryptoWatcher.Application.DTOs.Requests;

public record CreateAlertRequest(
    string CryptoSymbol,
    decimal TargetPrice,
    PriceCondition Condition
);