using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RecebimentoMercadorias.Data;
using RecebimentoMercadorias.Security;
using RecebimentoMercadorias.Services;

var builder = WebApplication.CreateBuilder(args);

var supabaseUrl = builder.Configuration["SUPABASE_URL"]?.TrimEnd('/')
    ?? throw new InvalidOperationException("Configure SUPABASE_URL para validar tokens do Supabase.");

var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

// Adiciona os controllers do projeto
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();

// Injeção de dependência dos serviços
builder.Services.AddScoped<FornecedorService>();
builder.Services.AddScoped<CategoriaService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<ProdutoService>();
builder.Services.AddScoped<PedidoService>();
builder.Services.AddScoped<MovimentacaoEstoqueService>();
builder.Services.AddTransient<IClaimsTransformation, StockUserClaimsTransformation>();
builder.Services.AddHttpClient<SupabaseAdminClient>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"{supabaseUrl}/auth/v1";
        options.RequireHttpsMetadata = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"{supabaseUrl}/auth/v1",
            ValidateAudience = true,
            ValidAudience = "authenticated",
            ValidateLifetime = true
        };
    });

var stockUserPolicy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .RequireClaim("stock_access", "true");

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = stockUserPolicy.Build();
    options.FallbackPolicy = stockUserPolicy.Build();
    options.AddPolicy("Managers", policy =>
        policy.RequireAuthenticatedUser()
            .RequireClaim("stock_access", "true")
            .RequireRole("GERENTE", "CEO"));
    options.AddPolicy("CEO", policy =>
        policy.RequireAuthenticatedUser()
            .RequireClaim("stock_access", "true")
            .RequireRole("CEO"));
    options.AddPolicy("OrderCreators", policy =>
        policy.RequireAuthenticatedUser()
            .RequireClaim("stock_access", "true")
            .RequireRole("FILIAL", "GERENTE", "CEO"));
    options.AddPolicy("ConferenceOperators", policy =>
        policy.RequireAuthenticatedUser()
            .RequireClaim("stock_access", "true")
            .RequireRole("CONFERENTE", "GERENTE", "CEO"));
    options.AddPolicy("StockOperators", policy =>
        policy.RequireAuthenticatedUser()
            .RequireClaim("stock_access", "true")
            .RequireRole("FILIAL", "CONFERENTE", "GERENTE", "CEO"));
});

// Configuração do Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Controle de Estoque API",
        Version = "v1",
        Description = "API para gestão de fornecedores, produtos com leitor de código de barras e estoque de supermercado."
    });

    options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Token de acesso emitido pelo Supabase Auth."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

var allowedOrigins = builder.Configuration["CORS_ALLOWED_ORIGINS"]?
    .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    .Where(origin => !string.IsNullOrWhiteSpace(origin))
    .ToArray()
    ?? ["https://interface-cypx.vercel.app", "http://localhost:3000"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("Application", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Configure ConnectionStrings__DefaultConnection.");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// API apenas: sem arquivos estáticos nem interface web
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Controle de Estoque API v1");
    c.RoutePrefix = "swagger";
});

app.MapGet("/", () => Results.Redirect("/swagger"));

app.UseHttpsRedirection();
app.UseCors("Application");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();