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

// Registrar Infrastructure (DbContext, Repositories, Services)
builder.Services.AddInfrastructure(builder.Configuration);

// Registrar Use Cases
builder.Services.AddScoped<CreateUserUseCase>();
builder.Services.AddScoped<CreateAlertUseCase>();
builder.Services.AddScoped<GetActiveAlertsUseCase>();

var app = builder.Build();

// ? APLICAR MIGRATIONS AUTOMATICAMENTE
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Aplicando migrations...");
        context.Database.Migrate(); // Aplica todas as migrations pendentes
        logger.LogInformation("? Migrations aplicadas com sucesso!");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "? Erro ao aplicar migrations");
        throw; // Falha a aplicação se não conseguir aplicar migrations
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