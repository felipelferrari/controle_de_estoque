namespace RecebimentoMercadorias.Entities;

public enum TipoMovimentacao
{
    Entrada = 1,
    Saida = 2,
    PerdaAvaria = 3
}

public sealed class MovimentacaoEstoque
{
    public int Id { get; set; }
    public int ProdutoId { get; set; }
    public Produto? Produto { get; set; }
    public int Quantidade { get; set; }
    public TipoMovimentacao Tipo { get; set; }
    public DateTime DataMovimentacao { get; set; } = DateTime.UtcNow;
    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
}