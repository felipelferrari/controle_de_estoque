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

        modelBuilder.Entity<Fornecedor>(entity =>
        {
            entity.ToTable("fornecedores");
            entity.HasKey(f => f.Id);
            entity.Property(f => f.Id).HasColumnName("id_fornecedor").ValueGeneratedOnAdd();
            entity.Property(f => f.RazaoSocial).HasColumnName("razao_social").HasMaxLength(150).IsRequired();
            entity.Property(f => f.NomeFantasia).HasColumnName("nome_fantasia").HasMaxLength(150);
            entity.Property(f => f.Cnpj).HasColumnName("cnpj").HasMaxLength(18).IsRequired();
            entity.HasIndex(f => f.Cnpj).IsUnique();
            entity.Property(f => f.Telefone).HasColumnName("telefone").HasMaxLength(20);
            entity.Property(f => f.Email).HasColumnName("email").HasMaxLength(100);
            entity.Property(f => f.Endereco).HasColumnName("endereco").HasMaxLength(200);
            entity.Property(f => f.Ativo).HasColumnName("ativo").HasDefaultValue(true);
            entity.Property(f => f.DataCadastro).HasColumnName("data_cadastro").HasDefaultValueSql("now()");
        });

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