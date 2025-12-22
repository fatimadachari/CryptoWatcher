using CryptoWatcher.Domain.Common;
using CryptoWatcher.Domain.Enums;

namespace CryptoWatcher.Domain.Entities;

public class CryptoAlert : BaseEntity
{
    public int UserId { get; private set; }
    public string CryptoSymbol { get; private set; }  // Ex: "BTC", "ETH"
    public decimal TargetPrice { get; private set; }
    public PriceCondition Condition { get; private set; }
    public AlertStatus Status { get; private set; }
    public DateTime? TriggeredAt { get; private set; }

    // Relacionamento: N Alerts pertencem a 1 User
    public User User { get; private set; } = null!;

    // Construtor privado para EF Core
    private CryptoAlert() { }

    // Construtor de criação
    public CryptoAlert(
        int userId,
        string cryptoSymbol,
        decimal targetPrice,
        PriceCondition condition)
    {
        ValidateCryptoSymbol(cryptoSymbol);
        ValidateTargetPrice(targetPrice);

        UserId = userId;
        CryptoSymbol = cryptoSymbol.ToUpper(); // Sempre maiúsculo
        TargetPrice = targetPrice;
        Condition = condition;
        Status = AlertStatus.Active; // Todo alerta começa ativo
    }

    // ⭐ MÉTODO DE DOMÍNIO PRINCIPAL
    public bool ShouldTrigger(decimal currentPrice)
    {
        if (Status != AlertStatus.Active)
            return false;

        return Condition switch
        {
            PriceCondition.Above => currentPrice >= TargetPrice,
            PriceCondition.Below => currentPrice <= TargetPrice,
            _ => false
        };
    }

    // Marca o alerta como disparado
    public void MarkAsTriggered()
    {
        if (Status != AlertStatus.Active)
            throw new InvalidOperationException("Apenas alertas ativos podem ser disparados");

        Status = AlertStatus.Triggered;
        TriggeredAt = DateTime.UtcNow;
        SetUpdatedAt();
    }

    // Cancela o alerta
    public void Cancel()
    {
        if (Status == AlertStatus.Triggered)
            throw new InvalidOperationException("Não é possível cancelar um alerta já disparado");

        Status = AlertStatus.Cancelled;
        SetUpdatedAt();
    }

    // Validações privadas
    private static void ValidateCryptoSymbol(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException("Símbolo da cripto não pode ser vazio", nameof(symbol));

        if (symbol.Length is < 2 or > 10)
            throw new ArgumentException("Símbolo deve ter entre 2 e 10 caracteres", nameof(symbol));
    }

    private static void ValidateTargetPrice(decimal price)
    {
        if (price <= 0)
            throw new ArgumentException("Preço alvo deve ser maior que zero", nameof(price));
    }
}