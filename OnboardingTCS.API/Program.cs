using OnboardingTCS.Core.Core.Interfaces;
using OnboardingTCS.Core.Infrastructure.Data;
using OnboardingTCS.Core.Infrastructure.Repositories;
using OnboardingTCS.Core.Interfaces;                     // de dev_premaster
using OnboardingTCS.Core.Infrastructure.Services;        // de master
using OnboardingTCS.Core.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register MongoDbContext
builder.Services.AddScoped<MongoDbContext>();

// Register repositories
builder.Services.AddScoped<ISupervisorRepository, SupervisorRepository>();

// 🔹 Servicios de dev_premaster
builder.Services.AddScoped<IActividadesRepository, ActividadesRepository>();
builder.Services.AddScoped<IMensajesAutomaticosRepository, MensajesAutomaticosRepository>();
builder.Services.AddScoped<ILikesCursosRepository, LikesCursosRepository>();
builder.Services.AddScoped<IMensajesEnviadosRepository, MensajesEnviadosRepository>();

// 🔹 Servicios de master
builder.Services.AddScoped<IDocumentoRepository, DocumentoRepository>();
builder.Services.AddHttpClient<IOllamaService, OllamaService>();

// Register HistorialChatRepository
builder.Services.AddScoped<HistorialChatRepository>();

// Register CursoRepository
builder.Services.AddScoped<CursoRepository>();

// Register UsuarioRepository
builder.Services.AddScoped<UsuarioRepository>();

// Register SupervisorService
builder.Services.AddScoped<SupervisorService>();

// OpenAPI
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
