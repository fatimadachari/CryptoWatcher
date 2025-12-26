using CryptoWatcher.Application.UseCases.Alerts;
using CryptoWatcher.Application.UseCases.Users;
using CryptoWatcher.Infrastructure;
using CryptoWatcher.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrar Infrastructure SOMENTE se NÃO for Testing
if (builder.Environment.EnvironmentName != "Testing")
{
    builder.Services.AddInfrastructure(builder.Configuration);
}

// Registrar Use Cases
builder.Services.AddScoped<CreateUserUseCase>();
builder.Services.AddScoped<CreateAlertUseCase>();
builder.Services.AddScoped<GetActiveAlertsUseCase>();

var app = builder.Build();

// ? APLICAR MIGRATIONS AUTOMATICAMENTE (somente se não for Testing)
if (app.Environment.EnvironmentName != "Testing")
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<AppDbContext>();
            var logger = services.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("Aplicando migrations...");
            context.Database.Migrate();
            logger.LogInformation("? Migrations aplicadas com sucesso!");
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "? Erro ao aplicar migrations");
            throw;
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Tornar a classe Program acessível para testes
public partial class Program { }