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

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuarios");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id).HasColumnName("id_usuario").ValueGeneratedOnAdd();
            entity.Property(u => u.Nome).HasColumnName("nome").HasMaxLength(150).IsRequired();
            entity.Property(u => u.Email).HasColumnName("email").HasMaxLength(100).IsRequired();
            entity.Property(u => u.Cargo).HasColumnName("cargo").HasMaxLength(80).IsRequired();
            entity.Property(u => u.Ativo).HasColumnName("ativo").HasDefaultValue(true);
            entity.Property(u => u.DataCadastro).HasColumnName("data_cadastro").HasDefaultValueSql("now()");
        });

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

        modelBuilder.Entity<Produto>(entity =>
        {
            entity.ToTable("produtos");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id).HasColumnName("id_produto").ValueGeneratedOnAdd();
            entity.Property(p => p.CodigoBarras).HasColumnName("codigo_barras").HasMaxLength(50);
            entity.HasIndex(p => p.CodigoBarras).IsUnique();
            entity.Property(p => p.Nome).HasColumnName("nome").HasMaxLength(100).IsRequired();
            entity.Property(p => p.Descricao).HasColumnName("descricao");
            entity.Property(p => p.PrecoCusto).HasColumnName("preco_custo").HasColumnType("numeric(10,2)").IsRequired();
            entity.Property(p => p.PrecoVenda).HasColumnName("preco_venda").HasColumnType("numeric(10,2)").IsRequired();
            entity.Property(p => p.IdCategoria).HasColumnName("id_categoria");
            entity.Property(p => p.IdFornecedor).HasColumnName("id_fornecedor");
            entity.Property(p => p.Ativo).HasColumnName("ativo").HasDefaultValue(true);
            entity.Property(p => p.DataCadastro).HasColumnName("data_cadastro").HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<MovimentacaoEstoque>(entity =>
        {
            entity.ToTable("movimentacoes_estoque");
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Id).HasColumnName("id_movimentacao").ValueGeneratedOnAdd();
            entity.Property(m => m.FilialId).HasColumnName("id_filial").IsRequired();
            entity.Property(m => m.ProdutoId).HasColumnName("id_produto").IsRequired();
            entity.Property(m => m.Tipo)
                  .HasColumnName("tipo_movimentacao")
                  .HasConversion<string>()
                  .HasMaxLength(50)
                  .IsRequired();
            entity.Property(m => m.Quantidade).HasColumnName("quantidade").IsRequired();
            entity.Property(m => m.DataMovimentacao).HasColumnName("data_movimentacao").HasDefaultValueSql("now()");
            entity.Property(m => m.Observacao).HasColumnName("observacao");

            entity.HasOne(m => m.Produto)
                  .WithMany()
                  .HasForeignKey(m => m.ProdutoId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}