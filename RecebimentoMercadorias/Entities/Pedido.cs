using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RecebimentoMercadorias.Entities;

public enum StatusPedido
{
    Aberto = 1,
    EmConferencia = 2,
    Confirmado = 3,
    Cancelado = 4
}

[Table("pedidos")]
public sealed class Pedido
{
    [Column("id_pedido")]
    public int Id { get; set; }

    [Column("numero_pedido")]
    public string NumeroPedido { get; set; } = string.Empty;

    [Column("id_filial")]
    public int FilialId { get; set; }

    [Column("status")]
    public StatusPedido Status { get; set; } = StatusPedido.Aberto;

    [Column("valor_total")]
    public decimal ValorTotal { get; set; }

    [Column("data_pedido")]
    public DateTime DataPedido { get; set; } = DateTime.UtcNow;

    [Column("observacao")]
    public string? Observacao { get; set; }

    [JsonIgnore]
    public ICollection<PedidoItem> Itens { get; set; } = new List<PedidoItem>();
}

[Table("pedido_itens")]
public sealed class PedidoItem
{
    [Column("id_pedido_item")]
    public int Id { get; set; }

    [Column("id_pedido")]
    public int PedidoId { get; set; }

    [JsonIgnore]
    public Pedido Pedido { get; set; } = null!;

    [Column("id_produto")]
    public int ProdutoId { get; set; }

    public Produto Produto { get; set; } = null!;

    [Column("quantidade")]
    public int Quantidade { get; set; }

    [Column("preco_unitario")]
    public decimal PrecoUnitario { get; set; }

    [Column("total")]
    public decimal Total { get; set; }

    [Column("observacao")]
    public string? Observacao { get; set; }
}

[Table("conferencias")]
public sealed class ConferenciaPedido
{
    [Column("id_conferencia")]
    public int Id { get; set; }

    [Column("id_pedido")]
    public int PedidoId { get; set; }

    [JsonIgnore]
    public Pedido Pedido { get; set; } = null!;

    [Column("id_produto")]
    public int ProdutoId { get; set; }

    public Produto Produto { get; set; } = null!;

    [Column("quantidade_recebida")]
    public int QuantidadeRecebida { get; set; }

    [Column("quantidade_esperada")]
    public int QuantidadeEsperada { get; set; }

    [Column("status")]
    public string Status { get; set; } = "Pendente";

    [Column("observacao")]
    public string? Observacao { get; set; }

    [Column("data_conferencia")]
    public DateTime DataConferencia { get; set; } = DateTime.UtcNow;
}
