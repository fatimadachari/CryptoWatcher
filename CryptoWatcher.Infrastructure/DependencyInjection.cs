using CryptoWatcher.Application.Interfaces.Repositories;
using CryptoWatcher.Application.Interfaces.Services;
using CryptoWatcher.Infrastructure.Data;
using CryptoWatcher.Infrastructure.Repositories;
using CryptoWatcher.Infrastructure.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using StackExchange.Redis;

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

        // Redis
        var redisConnection = configuration.GetConnectionString("Redis") ?? "localhost:6379";
        services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(redisConnection)
        );
        services.AddSingleton<ICacheService, RedisCacheService>();

        // Configurar HttpClient para CoinGecko com Polly
        services.AddHttpClient<CoinGeckoPriceService>(client =>
        {
            client.BaseAddress = new Uri("https://api.coingecko.com/api/v3/");
            client.DefaultRequestHeaders.Add("User-Agent", "CryptoWatcher/1.0");
            client.Timeout = TimeSpan.FromSeconds(30);
        })
        .AddPolicyHandler(GetRetryPolicy())
        .AddPolicyHandler(GetCircuitBreakerPolicy());

        // Registrar PriceService com cache (Decorator Pattern)
        services.AddScoped<IPriceService, CachedPriceService>();

        // Registrar PriceService com cache (Decorator Pattern)
        services.AddScoped<IPriceService, CachedPriceService>();

        // RabbitMQ com MassTransit
        services.AddMassTransit(config =>
        {
            config.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMqHost = configuration.GetValue<string>("RabbitMq:Host") ?? "localhost";
                var rabbitMqUser = configuration.GetValue<string>("RabbitMq:Username") ?? "admin";
                var rabbitMqPass = configuration.GetValue<string>("RabbitMq:Password") ?? "admin123";

                cfg.Host(rabbitMqHost, "/", h =>
                {
                    h.Username(rabbitMqUser);
                    h.Password(rabbitMqPass);
                });
            });
        });

        services.AddScoped<IMessagePublisher, RabbitMqPublisher>();

        return services;
    }

    // Policy 1: Retry com Backoff Exponencial
    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
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
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30),
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