using System.ComponentModel.DataAnnotations.Schema;

namespace RecebimentoMercadorias.Entities;

public sealed class Fornecedor
{
    [Column("id_fornecedor")]
    public int Id { get; set; }

    [Column("razao_social")]
    public string RazaoSocial { get; set; } = string.Empty;

    [Column("nome_fantasia")]
    public string? NomeFantasia { get; set; }

    [Column("cnpj")]
    public string Cnpj { get; set; } = string.Empty;

    [Column("telefone")]
    public string? Telefone { get; set; }

    [Column("email")]
    public string? Email { get; set; }

    [Column("endereco")]
    public string? Endereco { get; set; }

    [Column("ativo")]
    public bool Ativo { get; set; } = true;

    [Column("data_cadastro")]
    public DateTime? DataCadastro { get; set; } = DateTime.UtcNow;

    // Relacionamento 1-para-Muitos com os Produtos do supermercado
    public ICollection<Produto> Produtos { get; set; } = new List<Produto>();
}