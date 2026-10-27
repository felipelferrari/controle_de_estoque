using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RecebimentoMercadorias.Data;

var builder = WebApplication.CreateBuilder(args);

// Adiciona os controllers do projeto
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configuração do Swagger com prevenção contra conflito de DTOs e Namespaces
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Controle de Estoque API",
        Version = "v1",
        Description = "API para gestão de fornecedores, produtos com leitor de código de barras e estoque de supermercado."
    });

    // Resolve o conflito entre Entities.Fornecedor e Services.Fornecedor
    options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
});

// Configuração do CORS para permitir requisições do frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Banco de dados em memória para execução e testes rápidos
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("EstoqueDb"));

var app = builder.Build();

// SWAGGER ATIVO EM TODOS OS AMBIENTES (Sem a condição 'if (app.Environment.IsDevelopment())')
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Controle de Estoque API v1");
    c.RoutePrefix = string.Empty; // Define a rota raiz (http://localhost:5000) como o Swagger
});

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();

app.MapControllers();

app.Run();