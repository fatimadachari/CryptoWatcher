using CryptoWatcher.Infrastructure;
using CryptoWatcher.Worker.Services;
using CryptoWatcher.Worker.Workers;

var builder = Host.CreateApplicationBuilder(args);

// Registrar Infrastructure (DbContext, Repositories, Services)
builder.Services.AddInfrastructure(builder.Configuration);

// Registrar o serviço de monitoramento
builder.Services.AddScoped<PriceMonitorService>();

// Registrar o Worker
builder.Services.AddHostedService<PriceMonitorWorker>();

var host = builder.Build();
host.Run();