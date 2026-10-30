using System.ComponentModel.DataAnnotations.Schema;

namespace RecebimentoMercadorias.Entities;

public sealed class Produto
{
    [Column("id_produto")]
    public int Id { get; set; }

    [Column("codigo_barras")]
    public string? CodigoBarras { get; set; }

    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [Column("descricao")]
    public string? Descricao { get; set; }

    [Column("preco_custo")]
    public decimal PrecoCusto { get; set; }

    [Column("preco_venda")]
    public decimal PrecoVenda { get; set; }

    [Column("id_categoria")]
    public int? IdCategoria { get; set; }

    [Column("id_fornecedor")]
    public int? IdFornecedor { get; set; }

    [Column("ativo")]
    public bool Ativo { get; set; } = true;

    [Column("data_cadastro")]
    public DateTime? DataCadastro { get; set; } = DateTime.UtcNow;
}