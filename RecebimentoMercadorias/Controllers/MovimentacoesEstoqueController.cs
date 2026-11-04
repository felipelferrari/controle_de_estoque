using Microsoft.AspNetCore.Mvc;
using RecebimentoMercadorias.Entities;
using RecebimentoMercadorias.Services;

namespace RecebimentoMercadorias.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class MovimentacoesEstoqueController : ControllerBase
{
    private readonly MovimentacaoEstoqueService _movimentacaoService;

    public MovimentacoesEstoqueController(MovimentacaoEstoqueService movimentacaoService)
    {
        _movimentacaoService = movimentacaoService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<MovimentacaoEstoque>> Get()
    {
        return Ok(_movimentacaoService.ObterTodos());
    }

    [HttpGet("{id:int}")]
    public ActionResult<MovimentacaoEstoque> GetById(int id)
    {
        var movimentacao = _movimentacaoService.ObterPorId(id);
        if (movimentacao is null) return NotFound();
        return Ok(movimentacao);
    }

    [HttpGet("produto/{produtoId:int}")]
    public ActionResult<IEnumerable<MovimentacaoEstoque>> GetByProduto(int produtoId)
    {
        return Ok(_movimentacaoService.ObterPorProduto(produtoId));
    }

    [HttpPost]
    public ActionResult<MovimentacaoEstoque> Post([FromBody] RegistrarMovimentacaoRequest request)
    {
        try
        {
            var movimentacao = _movimentacaoService.Registrar(
                request.ProdutoId,
                request.FilialId,
                request.Tipo,
                request.Quantidade,
                request.Observacao);

            return CreatedAtAction(nameof(GetById), new { id = movimentacao.Id }, movimentacao);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

public sealed class RegistrarMovimentacaoRequest
{
    public int ProdutoId { get; set; }
    public int FilialId { get; set; }
    public TipoMovimentacao Tipo { get; set; }
    public int Quantidade { get; set; }
    public string? Observacao { get; set; }
}
