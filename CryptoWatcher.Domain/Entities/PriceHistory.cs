using CryptoWatcher.Domain.Common;

namespace CryptoWatcher.Domain.Entities;

public class PriceHistory : BaseEntity
{
    public string CryptoSymbol { get; private set; }
    public decimal Price { get; private set; }
    public string Source { get; private set; } // Ex: "CoinGecko"
    public DateTime FetchedAt { get; private set; }

    private PriceHistory() { }

    public PriceHistory(string cryptoSymbol, decimal price, string source)
    {
        if (string.IsNullOrWhiteSpace(cryptoSymbol))
            throw new ArgumentException("Símbolo não pode ser vazio", nameof(cryptoSymbol));

        if (price <= 0)
            throw new ArgumentException("Preço deve ser maior que zero", nameof(price));

        CryptoSymbol = cryptoSymbol.ToUpper();
        Price = price;
        Source = source;
        FetchedAt = DateTime.UtcNow;
    }
}