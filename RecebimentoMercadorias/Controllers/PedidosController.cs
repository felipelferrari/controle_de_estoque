using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecebimentoMercadorias.Entities;
using RecebimentoMercadorias.Services;

namespace RecebimentoMercadorias.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "StockUser")]
public sealed class PedidosController : ControllerBase
{
    private readonly PedidoService _pedidoService;

    public PedidosController(PedidoService pedidoService)
    {
        _pedidoService = pedidoService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Pedido>> Get()
    {
        return Ok(_pedidoService.ObterTodos());
    }

    [HttpGet("{id:int}")]
    public ActionResult<Pedido> GetById(int id)
    {
        var pedido = _pedidoService.ObterPorId(id);
        if (pedido is null) return NotFound();
        return Ok(pedido);
    }

    [HttpPost]
    [Authorize(Policy = "OrderCreators")]
    public ActionResult<Pedido> Post([FromBody] CreatePedidoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.NumeroPedido))
            return BadRequest("Número do pedido é obrigatório.");

        var pedido = _pedidoService.Criar(request.NumeroPedido, request.FilialId, request.Observacao);
        return CreatedAtAction(nameof(GetById), new { id = pedido.Id }, pedido);
    }

    [HttpPost("{id:int}/itens")]
    [Authorize(Policy = "OrderCreators")]
    public ActionResult<Pedido> AdicionarItem(int id, [FromBody] AddPedidoItemRequest request)
    {
        try
        {
            var pedido = _pedidoService.AdicionarItem(id, request.ProdutoId, request.Quantidade, request.PrecoUnitario, request.Observacao);
            if (pedido is null) return NotFound();
            return Ok(pedido);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPatch("{id:int}/status")]
    [Authorize(Policy = "ConferenceOperators")]
    public ActionResult<Pedido> AtualizarStatus(int id, [FromBody] UpdatePedidoStatusRequest request)
    {
        var pedido = _pedidoService.AtualizarStatus(id, request.Status);
        if (pedido is null) return NotFound();
        return Ok(pedido);
    }

    [HttpGet("{id:int}/conferencia")]
    [Authorize(Policy = "ConferenceOperators")]
    public ActionResult<IEnumerable<ConferenciaPedido>> GetConferencia(int id)
    {
        return Ok(_pedidoService.ObterConferenciaDoPedido(id));
    }

    [HttpPost("{id:int}/conferencia")]
    [Authorize(Policy = "ConferenceOperators")]
    public ActionResult RegistrarConferencia(int id, [FromBody] RegistrarConferenciaRequest request)
    {
        try
        {
            _pedidoService.RegistrarConferencia(id, request.ProdutoId, request.QuantidadeRecebida, request.Observacao);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

public sealed class CreatePedidoRequest
{
    public string NumeroPedido { get; set; } = string.Empty;
    public int FilialId { get; set; }
    public string? Observacao { get; set; }
}

public sealed class AddPedidoItemRequest
{
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public string? Observacao { get; set; }
}

public sealed class UpdatePedidoStatusRequest
{
    public StatusPedido Status { get; set; }
}

public sealed class RegistrarConferenciaRequest
{
    public int ProdutoId { get; set; }
    public int QuantidadeRecebida { get; set; }
    public string? Observacao { get; set; }
}
