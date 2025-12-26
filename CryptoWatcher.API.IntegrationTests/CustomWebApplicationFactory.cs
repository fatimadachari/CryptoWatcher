using CryptoWatcher.Infrastructure;
using CryptoWatcher.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoWatcher.API.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Registrar InMemory usando o método de extensão
            services.AddInfrastructureWithInMemoryDb("IntegrationTestDb");

            // Garantir que o banco está criado
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        });

        // Usar environment de teste
        builder.UseEnvironment("Testing");
    }
}