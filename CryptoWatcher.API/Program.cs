using CryptoWatcher.Application.UseCases.Alerts;
using CryptoWatcher.Application.UseCases.Users;
using CryptoWatcher.Infrastructure;

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