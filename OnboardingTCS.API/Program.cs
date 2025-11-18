using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OnboardingTCS.Core.Core.Interfaces;
using OnboardingTCS.Core.Core.Settings;
using OnboardingTCS.Core.Infrastructure.Data;
using OnboardingTCS.Core.Infrastructure.Repositories;
using OnboardingTCS.Core.Interfaces;
using OnboardingTCS.Core.Infrastructure.Services;
using OnboardingTCS.Core.Core.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add HttpClient for OllamaService con timeout extendido para IA
builder.Services.AddHttpClient<IOllamaService, OllamaService>(client =>
{
    // ?? Timeout de 15 minutos para generar respuestas de IA (aumentado de 10 a 15 minutos)
    client.Timeout = TimeSpan.FromMinutes(15);
    
    // ?? Headers adicionales
    client.DefaultRequestHeaders.Add("User-Agent", "OnboardingTCS/1.0");
    
    // ?? Tamaño máximo de respuesta (100MB para modelos de IA, aumentado de 50MB)
    client.MaxResponseContentBufferSize = 100 * 1024 * 1024;
});

// Configure JWT Settings
builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("JWT"));

// Add Authentication & Authorization
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]!)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register MongoDbContext
builder.Services.AddScoped<MongoDbContext>();

// Register JWT Service
builder.Services.AddScoped<IJWTService, JWTService>();

// Register repositories
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ICursoRepository, CursoRepository>();
builder.Services.AddScoped<IActividadesRepository, ActividadesRepository>();
builder.Services.AddScoped<IDocumentoRepository, DocumentoRepository>();
builder.Services.AddScoped<IMensajesAutomaticosRepository, MensajesAutomaticosRepository>();
builder.Services.AddScoped<ILikesCursosRepository, LikesCursosRepository>();
builder.Services.AddScoped<ISupervisorRepository, SupervisorRepository>();
builder.Services.AddScoped<IHistorialChatRepository, HistorialChatRepository>();
builder.Services.AddScoped<IMensajesEnviadosRepository, MensajesEnviadosRepository>();

// Register services
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ICursoService, CursoService>();
builder.Services.AddScoped<IActividadesService, ActividadesService>();
builder.Services.AddScoped<IDocumentoService, DocumentoService>();
builder.Services.AddScoped<IMensajesAutomaticosService, MensajesAutomaticosService>();
builder.Services.AddScoped<ILikesCursosService, LikesCursosService>();
builder.Services.AddScoped<ISupervisorService, SupervisorService>();
builder.Services.AddScoped<IHistorialChatService, HistorialChatService>();
// ?? NOTA: IOllamaService ya está registrado arriba con AddHttpClient, no duplicar aquí
builder.Services.AddScoped<IMensajesEnviadosService, MensajesEnviadosService>();

// OpenAPI with JWT Support
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "OnboardingTCS API",
        Version = "v1",
        Description = "API for TCS Onboarding System with JWT Authentication"
    });

    // Add JWT authentication support to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "OnboardingTCS API v1");
    });
}

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
