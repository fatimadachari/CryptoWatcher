using CryptoWatcher.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace CryptoWatcher.Infrastructure.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDatabase _database;
    private readonly ILogger<RedisCacheService> _logger;

    public RedisCacheService(IConnectionMultiplexer redis, ILogger<RedisCacheService> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
    }

    public async Task<string?> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var value = await _database.StringGetAsync(key);

            if (value.HasValue)
            {
                _logger.LogInformation("✅ Cache HIT: {Key}", key);
                return value.ToString();
            }

            _logger.LogInformation("❌ Cache MISS: {Key}", key);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao ler do Redis: {Key}", key);
            return null;
        }
    }

    public async Task SetAsync(
        string key,
        string value,
        TimeSpan expiration,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _database.StringSetAsync(key, value, expiration);
            _logger.LogInformation("💾 Cache SET: {Key} (TTL: {Expiration}s)", key, expiration.TotalSeconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao escrever no Redis: {Key}", key);
        }
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _database.KeyExistsAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar existência no Redis: {Key}", key);
            return false;
        }
    }
}