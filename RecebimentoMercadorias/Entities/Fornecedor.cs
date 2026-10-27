namespace RecebimentoMercadorias.Entities;

public sealed class Fornecedor
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

    // Relacionamento 1-para-Muitos com os Produtos do supermercado
    public ICollection<Produto> Produtos { get; set; } = new List<Produto>();
}