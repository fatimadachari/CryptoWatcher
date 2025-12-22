namespace CryptoWatcher.Application.Interfaces.Services;

public interface IPriceService
{
    Task<decimal?> GetCurrentPriceAsync(string cryptoSymbol, CancellationToken cancellationToken = default);
}