using CryptoWatcher.Application.Interfaces.Services;
using System.Text.Json;

namespace CryptoWatcher.Infrastructure.Services;

public class CoinGeckoPriceService : IPriceService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://api.coingecko.com/api/v3";

    public CoinGeckoPriceService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(BaseUrl);
    }

    public async Task<decimal?> GetCurrentPriceAsync(
        string cryptoSymbol,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Mapear símbolo para ID do CoinGecko
            var coinId = MapSymbolToCoinId(cryptoSymbol);

            // Endpoint: /simple/price?ids=bitcoin&vs_currencies=usd
            var response = await _httpClient.GetAsync(
                $"/simple/price?ids={coinId}&vs_currencies=usd",
                cancellationToken
            );

            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var jsonDoc = JsonDocument.Parse(content);

            // Estrutura: { "bitcoin": { "usd": 50000 } }
            if (jsonDoc.RootElement.TryGetProperty(coinId, out var coinData))
            {
                if (coinData.TryGetProperty("usd", out var priceElement))
                {
                    return priceElement.GetDecimal();
                }
            }

            return null;
        }
        catch (Exception)
        {
            // Log aqui (faremos depois)
            return null;
        }
    }

    // Mapear símbolos comuns para IDs do CoinGecko
    private static string MapSymbolToCoinId(string symbol)
    {
        return symbol.ToUpper() switch
        {
            "BTC" => "bitcoin",
            "ETH" => "ethereum",
            "BNB" => "binancecoin",
            "SOL" => "solana",
            "ADA" => "cardano",
            "XRP" => "ripple",
            "DOT" => "polkadot",
            "DOGE" => "dogecoin",
            "AVAX" => "avalanche-2",
            "MATIC" => "matic-network",
            _ => symbol.ToLower() // Fallback
        };
    }
}