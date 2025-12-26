using CryptoWatcher.Application.Interfaces.Services;
using CryptoWatcher.Infrastructure;
using CryptoWatcher.Infrastructure.Services;
using CryptoWatcher.Worker.Consumers;
using CryptoWatcher.Worker.Services;
using CryptoWatcher.Worker.Workers;
using MassTransit;

var builder = Host.CreateApplicationBuilder(args);

// Registrar Infrastructure (DbContext, Repositories, Services)
builder.Services.AddInfrastructure(builder.Configuration);

// Registrar o serviço de monitoramento
builder.Services.AddScoped<PriceMonitorService>();

// Registrar RabbitMqPublisher (apenas no Worker)
builder.Services.AddScoped<IMessagePublisher, RabbitMqPublisher>();

// Configurar MassTransit com Consumer
builder.Services.AddMassTransit(config =>
{
    // Registrar o Consumer
    config.AddConsumer<AlertTriggeredConsumer>();

    config.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMqHost = builder.Configuration.GetValue<string>("RabbitMq:Host") ?? "localhost";
        var rabbitMqUser = builder.Configuration.GetValue<string>("RabbitMq:Username") ?? "admin";
        var rabbitMqPass = builder.Configuration.GetValue<string>("RabbitMq:Password") ?? "admin123";

        cfg.Host(rabbitMqHost, "/", h =>
        {
            h.Username(rabbitMqUser);
            h.Password(rabbitMqPass);
        });

        // Configurar endpoint do consumer
        cfg.ReceiveEndpoint("alert-triggered-queue", e =>
        {
            e.ConfigureConsumer<AlertTriggeredConsumer>(context);

            // Configurações de retry
            e.UseMessageRetry(r => r.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)));
        });
    });
});

// Registrar o Worker
builder.Services.AddHostedService<PriceMonitorWorker>();

var host = builder.Build();
host.Run();