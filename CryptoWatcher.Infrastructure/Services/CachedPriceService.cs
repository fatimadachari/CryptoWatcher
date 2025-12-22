using CryptoWatcher.Application.Interfaces.Services;

namespace CryptoWatcher.Infrastructure.Services;

public class CachedPriceService : IPriceService
{
    private readonly CoinGeckoPriceService _priceService;
    private readonly ICacheService _cacheService;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromSeconds(30);

    public CachedPriceService(
        CoinGeckoPriceService priceService,
        ICacheService cacheService)
    {
        _priceService = priceService;
        _cacheService = cacheService;
    }

    public async Task<decimal?> GetCurrentPriceAsync(
        string cryptoSymbol,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"price:{cryptoSymbol.ToUpper()}";

        // 1. Tentar obter do cache
        var cachedPrice = await _cacheService.GetAsync(cacheKey, cancellationToken);
        if (cachedPrice is not null && decimal.TryParse(cachedPrice, out var price))
        {
            return price;
        }

        // 2. Cache miss: consultar API
        var currentPrice = await _priceService.GetCurrentPriceAsync(cryptoSymbol, cancellationToken);

        // 3. Se obteve o preço, salvar no cache
        if (currentPrice.HasValue)
        {
            await _cacheService.SetAsync(
                cacheKey,
                currentPrice.Value.ToString(),
                _cacheDuration,
                cancellationToken
            );
        }

        return currentPrice;
    }
}