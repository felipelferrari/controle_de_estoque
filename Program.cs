using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RecebimentoMercadorias.Data;
using RecebimentoMercadorias.Services; // <-- Certifique-se de incluir esta linha

var builder = WebApplication.CreateBuilder(args);

// Adiciona os controllers do projeto
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// REGISTRE OS SERVIÇOS AQUI PARA A INJEÇÃO DE DEPENDÊNCIA:
builder.Services.AddScoped<FornecedorService>();
builder.Services.AddScoped<UsuarioService>();

// Configuração do Swagger com prevenção contra conflito de DTOs e Namespaces
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

// Banco de dados em memória
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("EstoqueDb"));

var app = builder.Build();

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