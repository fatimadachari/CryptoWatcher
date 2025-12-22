using CryptoWatcher.Application.Interfaces.Repositories;
using CryptoWatcher.Application.Interfaces.Services;
using CryptoWatcher.Infrastructure.Data;
using CryptoWatcher.Infrastructure.Repositories;
using CryptoWatcher.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace CryptoWatcher.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
            )
        );

        // Repositórios
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAlertRepository, AlertRepository>();

        // Configurar HttpClient para CoinGecko com Polly
        services.AddHttpClient<IPriceService, CoinGeckoPriceService>(client =>
        {
            client.BaseAddress = new Uri("https://api.coingecko.com/api/v3/");
            client.DefaultRequestHeaders.Add("User-Agent", "CryptoWatcher/1.0");
            client.Timeout = TimeSpan.FromSeconds(30);
        })
        .AddPolicyHandler(GetRetryPolicy())
        .AddPolicyHandler(GetCircuitBreakerPolicy());

        return services;
    }

    // Policy 1: Retry com Backoff Exponencial
    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError() // 5xx, 408, network errors
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests) // 429
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryAttempt, context) =>
                {
                    Console.WriteLine(
                        $"⚠️ Tentativa {retryAttempt} falhou. Aguardando {timespan.TotalSeconds}s antes de tentar novamente..."
                    );
                }
            );
    }

    // Policy 2: Circuit Breaker
    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5, // Abre após 5 falhas consecutivas
                durationOfBreak: TimeSpan.FromSeconds(30), // Fica aberto por 30s
                onBreak: (outcome, duration) =>
                {
                    Console.WriteLine($"🔴 Circuit Breaker ABERTO por {duration.TotalSeconds}s");
                },
                onReset: () =>
                {
                    Console.WriteLine("🟢 Circuit Breaker FECHADO - voltando ao normal");
                }
            );
    }
}