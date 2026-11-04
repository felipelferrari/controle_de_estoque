using Microsoft.EntityFrameworkCore;
using RecebimentoMercadorias.Data;
using RecebimentoMercadorias.Entities;

namespace RecebimentoMercadorias.Services;

public sealed class PedidoService
{
    private readonly AppDbContext _context;

    public PedidoService(AppDbContext context)
    {
        _context = context;
    }

    public List<Pedido> ObterTodos()
    {
        return _context.Pedidos
            .Include(p => p.Itens)
            .ThenInclude(i => i.Produto)
            .OrderByDescending(p => p.DataPedido)
            .ToList();
    }

    public Pedido? ObterPorId(int id)
    {
        return _context.Pedidos
            .Include(p => p.Itens)
            .ThenInclude(i => i.Produto)
            .FirstOrDefault(p => p.Id == id);
    }

    public Pedido Criar(string numeroPedido, int filialId, string? observacao = null)
    {
        if (string.IsNullOrWhiteSpace(numeroPedido))
            throw new ArgumentException("Número do pedido é obrigatório.");

        var pedido = new Pedido
        {
            NumeroPedido = numeroPedido.Trim(),
            FilialId = filialId,
            Observacao = observacao,
            Status = StatusPedido.Aberto,
            ValorTotal = 0m,
            DataPedido = DateTime.UtcNow
        };

        _context.Pedidos.Add(pedido);
        _context.SaveChanges();
        return pedido;
    }

    public Pedido? AdicionarItem(int pedidoId, int produtoId, int quantidade, decimal precoUnitario, string? observacao = null)
    {
        var pedido = _context.Pedidos
            .Include(p => p.Itens)
            .FirstOrDefault(p => p.Id == pedidoId);

        if (pedido is null) return null;

        var produto = _context.Produtos.FirstOrDefault(p => p.Id == produtoId);
        if (produto is null)
            throw new InvalidOperationException("Produto não encontrado.");

        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero.");

        var item = new PedidoItem
        {
            PedidoId = pedido.Id,
            ProdutoId = produto.Id,
            Produto = produto,
            Quantidade = quantidade,
            PrecoUnitario = precoUnitario,
            Total = quantidade * precoUnitario,
            Observacao = observacao
        };

        pedido.Itens.Add(item);
        pedido.ValorTotal += item.Total;
        _context.SaveChanges();

        return pedido;
    }

    public Pedido? AtualizarStatus(int pedidoId, StatusPedido status)
    {
        var pedido = _context.Pedidos.FirstOrDefault(p => p.Id == pedidoId);
        if (pedido is null) return null;

        pedido.Status = status;
        _context.SaveChanges();
        return pedido;
    }

    public List<ConferenciaPedido> ObterConferenciaDoPedido(int pedidoId)
    {
        return _context.Set<ConferenciaPedido>()
            .Where(c => c.PedidoId == pedidoId)
            .OrderBy(c => c.ProdutoId)
            .ToList();
    }

    public void RegistrarConferencia(int pedidoId, int produtoId, int quantidadeRecebida, string? observacao = null)
    {
        var pedido = _context.Pedidos
            .Include(p => p.Itens)
            .FirstOrDefault(p => p.Id == pedidoId);

        if (pedido is null)
            throw new InvalidOperationException("Pedido não encontrado.");

        var item = pedido.Itens.FirstOrDefault(i => i.ProdutoId == produtoId);
        if (item is null)
            throw new InvalidOperationException("Produto não está no pedido.");

        var conferencia = new ConferenciaPedido
        {
            PedidoId = pedido.Id,
            ProdutoId = item.ProdutoId,
            QuantidadeRecebida = quantidadeRecebida,
            QuantidadeEsperada = item.Quantidade,
            Status = quantidadeRecebida == item.Quantidade ? "Conferido" : "Parcial",
            Observacao = observacao,
            DataConferencia = DateTime.UtcNow
        };

        _context.Set<ConferenciaPedido>().Add(conferencia);
        _context.SaveChanges();
    }
}
