using GestionAPI.Models;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Configura el contexto de la base de datos
builder.Services.AddDbContext<GestionDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configura Redis
builder.Services.AddStackExchangeRedisOutputCache(opciones =>
{
    opciones.Configuration = builder.Configuration.GetConnectionString("redis");
});

// Registrar IConnectionMultiplexer
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("redis"), true);
    return ConnectionMultiplexer.Connect(configuration);
});

builder.Services.AddOutputCache();

builder.Services.AddControllers();

// Configura Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty; 
    });
}

app.UseHttpsRedirection();
app.UseOutputCache();
app.UseAuthorization();

app.MapControllers();

app.Run();
