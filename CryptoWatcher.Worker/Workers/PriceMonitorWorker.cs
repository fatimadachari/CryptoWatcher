using CryptoWatcher.Worker.Services;

namespace CryptoWatcher.Worker.Workers;

public class PriceMonitorWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PriceMonitorWorker> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(1); // Executa a cada 1 minuto

    public PriceMonitorWorker(
        IServiceProvider serviceProvider,
        ILogger<PriceMonitorWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker iniciado em: {Time}", DateTimeOffset.Now);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Criar um scope para usar serviços Scoped (repositórios)
                using var scope = _serviceProvider.CreateScope();
                var monitorService = scope.ServiceProvider.GetRequiredService<PriceMonitorService>();

                await monitorService.MonitorPricesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro crítico no Worker");
            }

            // Aguardar intervalo antes da próxima execução
            _logger.LogInformation("Próxima execução em {Interval}", _interval);
            await Task.Delay(_interval, stoppingToken);
        }

        _logger.LogInformation("Worker finalizado em: {Time}", DateTimeOffset.Now);
    }
}