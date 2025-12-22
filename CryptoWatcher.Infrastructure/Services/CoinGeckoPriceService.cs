using CryptoWatcher.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CryptoWatcher.Infrastructure.Services;

public class CoinGeckoPriceService : IPriceService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CoinGeckoPriceService> _logger;

    public CoinGeckoPriceService(HttpClient httpClient, ILogger<CoinGeckoPriceService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<decimal?> GetCurrentPriceAsync(
        string cryptoSymbol,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var coinId = MapSymbolToCoinId(cryptoSymbol);
            var url = $"simple/price?ids={coinId}&vs_currencies=usd";

            _logger.LogInformation("Consultando preço de {Symbol} (ID: {CoinId})", cryptoSymbol, coinId);

            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Erro ao consultar {CoinId}: {StatusCode}",
                    coinId,
                    response.StatusCode
                );
                return null;
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var jsonDoc = JsonDocument.Parse(content);

            if (jsonDoc.RootElement.TryGetProperty(coinId, out var coinData))
            {
                if (coinData.TryGetProperty("usd", out var priceElement))
                {
                    var price = priceElement.GetDecimal();
                    _logger.LogInformation("{Symbol}: ${Price:N2}", cryptoSymbol, price);
                    return price;
                }
            }

            _logger.LogWarning("Preço não encontrado na resposta para {CoinId}", coinId);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consultar preço de {Symbol}", cryptoSymbol);
            return null;
        }
    }

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
            _ => symbol.ToLower()
        };
    }
}