using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RecebimentoMercadorias.Data;
using RecebimentoMercadorias.Services;

var builder = WebApplication.CreateBuilder(args);

// Adiciona os controllers do projeto
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Injeção de dependência dos serviços
builder.Services.AddScoped<FornecedorService>();
builder.Services.AddScoped<UsuarioService>();

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
});

// Configuração do CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// CONEXÃO COM O SUPABASE (Lê a 'DefaultConnection' configurada no appsettings.json)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// CRIA AS TABELAS NO SUPABASE AUTOMATICAMENTE SE ELAS NÃO EXISTIREM
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Controle de Estoque API v1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();

app.MapControllers();

app.Run();