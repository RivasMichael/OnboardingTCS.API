using OnboardingTCS.Core.Core.Interfaces;
using OnboardingTCS.Core.Infrastructure.Data;
using OnboardingTCS.Core.Infrastructure.Repositories;
using OnboardingTCS.Core.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Register MongoDbContext
builder.Services.AddSingleton<MongoDbContext>();

// Register repositories
builder.Services.AddScoped<ISupervisorRepository, SupervisorRepository>();
builder.Services.AddScoped<IActividadesRepository, ActividadesRepository>();
builder.Services.AddScoped<IMensajesAutomaticosRepository, MensajesAutomaticosRepository>();
builder.Services.AddScoped<ILikesCursosRepository, LikesCursosRepository>();
builder.Services.AddScoped<IMensajesEnviadosRepository, MensajesEnviadosRepository>();
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
