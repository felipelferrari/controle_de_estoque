using Microsoft.EntityFrameworkCore;
using RecebimentoMercadorias.Entities;

namespace RecebimentoMercadorias.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Fornecedor> Fornecedores => Set<Fornecedor>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<MovimentacaoEstoque> MovimentacoesEstoque => Set<MovimentacaoEstoque>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Cria um índice único para o Código de Barras (melhora a performance na leitura por BIP)
        modelBuilder.Entity<Produto>()
            .HasIndex(p => p.CodigoBarras)
            .IsUnique();

        // Configuração da precisão decimal para valores monetários
        modelBuilder.Entity<Produto>()
            .Property(p => p.PrecoCusto)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Produto>()
            .Property(p => p.PrecoVenda)
            .HasPrecision(18, 2);
    }
}