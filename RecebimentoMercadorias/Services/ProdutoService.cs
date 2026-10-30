using Microsoft.EntityFrameworkCore;
using RecebimentoMercadorias.Data;
using RecebimentoMercadorias.Entities;

namespace RecebimentoMercadorias.Services;

public sealed class ProdutoService
{
    private readonly AppDbContext _context;

    public ProdutoService(AppDbContext context)
    {
        _context = context;
    }

    public List<Produto> ObterTodos()
    {
        return _context.Produtos
            .AsNoTracking()
            .OrderBy(p => p.Nome)
            .ToList();
    }

    public Produto? ObterPorId(int id)
    {
        return _context.Produtos
            .FirstOrDefault(p => p.Id == id);
    }

    public Produto Criar(
        string nome,
        string? codigoBarras,
        string? descricao,
        decimal precoCusto,
        decimal precoVenda,
        int? idCategoria,
        int? idFornecedor,
        bool ativo = true)
    {
        if (precoCusto < 0)
            throw new ArgumentException("O preço de custo não pode ser negativo.");

        if (precoVenda < 0)
            throw new ArgumentException("O preço de venda não pode ser negativo.");

        var produto = new Produto
        {
            Nome = nome.Trim(),
            CodigoBarras = string.IsNullOrWhiteSpace(codigoBarras) ? null : codigoBarras.Trim(),
            Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim(),
            PrecoCusto = precoCusto,
            PrecoVenda = precoVenda,
            IdCategoria = idCategoria,
            IdFornecedor = idFornecedor,
            Ativo = ativo,
            DataCadastro = DateTime.UtcNow
        };

        _context.Produtos.Add(produto);
        _context.SaveChanges();

        return produto;
    }

    public Produto? Atualizar(
        int id,
        string nome,
        string? codigoBarras,
        string? descricao,
        decimal precoCusto,
        decimal precoVenda,
        int? idCategoria,
        int? idFornecedor,
        bool ativo)
    {
        var produto = _context.Produtos.FirstOrDefault(p => p.Id == id);

        if (produto is null) return null;

        if (precoCusto < 0)
            throw new ArgumentException("O preço de custo não pode ser negativo.");

        if (precoVenda < 0)
            throw new ArgumentException("O preço de venda não pode ser negativo.");

        produto.Nome = nome.Trim();
        produto.CodigoBarras = string.IsNullOrWhiteSpace(codigoBarras) ? null : codigoBarras.Trim();
        produto.Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
        produto.PrecoCusto = precoCusto;
        produto.PrecoVenda = precoVenda;
        produto.IdCategoria = idCategoria;
        produto.IdFornecedor = idFornecedor;
        produto.Ativo = ativo;

        _context.SaveChanges();

        return produto;
    }

    public bool Remover(int id)
    {
        var produto = _context.Produtos.FirstOrDefault(p => p.Id == id);

        if (produto is null) return false;

        _context.Produtos.Remove(produto);
        _context.SaveChanges();

        return true;
    }

    public int ObterQuantidadeAtivos()
    {
        return _context.Produtos.Count(p => p.Ativo);
    }
}
