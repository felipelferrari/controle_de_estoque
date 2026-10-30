using System.ComponentModel.DataAnnotations.Schema;

namespace RecebimentoMercadorias.Entities;

public enum TipoMovimentacao
{
    Entrada = 1,
    Saida = 2,
    PerdaAvaria = 3
}

[Table("movimentacoes_estoque")]
public sealed class MovimentacaoEstoque
{
    [Column("id_movimentacao")]
    public int Id { get; set; }

    [Column("id_filial")]
    public int FilialId { get; set; }

    [Column("id_produto")]
    public int ProdutoId { get; set; }

    [ForeignKey(nameof(ProdutoId))]
    public Produto? Produto { get; set; }

    [Column("tipo_movimentacao")]
    public TipoMovimentacao Tipo { get; set; }

    [Column("quantidade")]
    public int Quantidade { get; set; }

    [Column("data_movimentacao")]
    public DateTime DataMovimentacao { get; set; } = DateTime.UtcNow;

    [Column("observacao")]
    public string? Observacao { get; set; }
}