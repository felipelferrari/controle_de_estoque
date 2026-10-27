namespace RecebimentoMercadorias.Entities;

public sealed class Produto
{
    public int Id { get; set; }
    public string CodigoBarras { get; set; } = string.Empty; // EAN-13 / GTIN lido pelo leitor
    public string Nome { get; set; } = string.Empty;
    public decimal PrecoCusto { get; set; }
    public decimal PrecoVenda { get; set; }
    public int QuantidadeEstoque { get; set; }
    public int EstoqueMinimo { get; set; }
    public DateTime? DataValidade { get; set; }
    public bool Ativo { get; set; } = true;

    // Relacionamento com o Fornecedor
    public int FornecedorId { get; set; }
    public Fornecedor? Fornecedor { get; set; }
}