using Microsoft.EntityFrameworkCore;
using RecebimentoMercadorias.Data;
using RecebimentoMercadorias.Entities;

namespace RecebimentoMercadorias.Services;

public sealed class MovimentacaoEstoqueService
{
    private readonly AppDbContext _context;

    public MovimentacaoEstoqueService(AppDbContext context)
    {
        _context = context;
    }

    public List<MovimentacaoEstoque> ObterTodos()
    {
        return _context.MovimentacoesEstoque
            .Include(m => m.Produto)
            .OrderByDescending(m => m.DataMovimentacao)
            .ToList();
    }

    public MovimentacaoEstoque? ObterPorId(int id)
    {
        return _context.MovimentacoesEstoque
            .Include(m => m.Produto)
            .FirstOrDefault(m => m.Id == id);
    }

    public List<MovimentacaoEstoque> ObterPorProduto(int produtoId)
    {
        return _context.MovimentacoesEstoque
            .Include(m => m.Produto)
            .Where(m => m.ProdutoId == produtoId)
            .OrderByDescending(m => m.DataMovimentacao)
            .ToList();
    }

    public MovimentacaoEstoque Registrar(int produtoId, int filialId, TipoMovimentacao tipo, int quantidade, string? observacao = null)
    {
        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero.");

        var produto = _context.Produtos.FirstOrDefault(p => p.Id == produtoId);
        if (produto is null)
            throw new InvalidOperationException("Produto não encontrado.");

        var movimentacao = new MovimentacaoEstoque
        {
            ProdutoId = produto.Id,
            FilialId = filialId,
            Tipo = tipo,
            Quantidade = quantidade,
            Observacao = observacao,
            DataMovimentacao = DateTime.UtcNow,
            Produto = produto
        };

        _context.MovimentacoesEstoque.Add(movimentacao);
        _context.SaveChanges();

        return movimentacao;
    }
}
