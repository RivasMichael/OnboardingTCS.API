using OnboardingTCS.Core.Infrastructure.Data;
using OnboardingTCS.Core.Interfaces;
using OnboardingTCS.Core.Infrastructure.Repositories;
using OnboardingTCS.Core.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Register MongoDbContext
builder.Services.AddSingleton<MongoDbContext>();

// Register ISupervisorRepository
builder.Services.AddScoped<ISupervisorRepository, SupervisorRepository>();

// Register IDocumentoRepository
builder.Services.AddScoped<IDocumentoRepository, DocumentoRepository>();

// Register IOllamaService
builder.Services.AddHttpClient<IOllamaService, OnboardingTCS.Core.Infrastructure.Services.OllamaService>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
