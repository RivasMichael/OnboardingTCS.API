using OnboardingTCS.Core.Infrastructure.Data;

using OnboardingTCS.API.Extensions; // <-- La carpeta donde est�n tus m�dulos
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

var builder = WebApplication.CreateBuilder(args);

// --- 1. REGISTRO DE SERVICIOS ---

// Registra los Controladores
=======
using OnboardingTCS.Core.Infrastructure.Repositories;
using OnboardingTCS.Core.Interfaces;                     // de dev_premaster
using OnboardingTCS.Core.Infrastructure.Services;        // de master

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Registra el Contexto de Mongo (�Tu compa�ero ya lo hizo!)
builder.Services.AddSingleton<MongoDbContext>();


// --- �TU PARTE! Registra HttpClient (para Ollama) ---
builder.Services.AddHttpClient();

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

// OpenAPI
builder.Services.AddOpenApi();


// --- �TU PARTE! Configura CORS (para Quasar) ---
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:8080", "http://localhost:9000") // Puertos de Quasar
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// --- �ARREGLO DEL 404! Habilitando Swagger ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- 2. REGISTRO DE M�DULOS DE EQUIPO ---
// (Esto llama a los archivos en la carpeta "Extensions")

// �TU M�DULO! (Chat y Login)
builder.Services.AddChatModule();
builder.Services.AddAuthModule(); // <-- M�dulo para el Login

// M�dulos de tus compa�eros
builder.Services.AddMensajesModule();
builder.Services.AddActividadesModule();
builder.Services.AddSupervisoresModule();

// --- 3. CONSTRUIR Y EJECUTAR LA APP ---
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    // --- �ARREGLO DEL 404! Encendiendo la UI de Swagger ---
    app.UseSwagger();
    app.UseSwaggerUI();
}

// --- �ARREGLO DEL WARN y ERR_CONNECTION_REFUSED! ---
// Deshabilitamos esto porque nuestro servidor local corre en HTTP, no HTTPS.
// app.UseHttpsRedirection(); 

// Ensure routing is enabled before mapping endpoints
app.UseRouting();

app.UseCors(); // <-- �Activa CORS!
app.UseAuthorization();

// Map controllers via endpoints to avoid MapControllers issues in some hosting scenarios
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();