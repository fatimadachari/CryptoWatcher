using CryptoWatcher.Application.Interfaces.Repositories;
using CryptoWatcher.Application.Interfaces.Services;
using CryptoWatcher.Infrastructure.Data;
using CryptoWatcher.Infrastructure.Repositories;
using CryptoWatcher.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        // Configurar HttpClient para CoinGecko
        services.AddHttpClient<IPriceService, CoinGeckoPriceService>(client =>
        {
            client.BaseAddress = new Uri("https://api.coingecko.com/api/v3/");
            client.DefaultRequestHeaders.Add("User-Agent", "CryptoWatcher/1.0");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        return services;
    }
}