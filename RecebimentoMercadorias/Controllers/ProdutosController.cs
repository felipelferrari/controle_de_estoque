using Microsoft.AspNetCore.Mvc;
using RecebimentoMercadorias.Entities;
using RecebimentoMercadorias.Services;

namespace RecebimentoMercadorias.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProdutosController : ControllerBase
{
    private readonly ProdutoService _produtoService;

    public ProdutosController(ProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Produto>> Get()
    {
        return Ok(_produtoService.ObterTodos());
    }

    [HttpGet("{id:int}")]
    public ActionResult<Produto> GetById(int id)
    {
        var produto = _produtoService.ObterPorId(id);

        if (produto is null)
        {
            return NotFound();
        }

        return Ok(produto);
    }

    [HttpPost]
    public ActionResult<Produto> Post([FromBody] CreateProdutoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
        {
            return BadRequest("Nome do produto é obrigatório.");
        }

        if (request.PrecoCusto < 0 || request.PrecoVenda < 0)
        {
            return BadRequest("Preços não podem ser negativos.");
        }

        var produto = _produtoService.Criar(
            request.Nome,
            request.CodigoBarras,
            request.Descricao,
            request.PrecoCusto,
            request.PrecoVenda,
            request.IdCategoria,
            request.IdFornecedor,
            request.Ativo);

        return CreatedAtAction(nameof(GetById), new { id = produto.Id }, produto);
    }

    [HttpPut("{id:int}")]
    public ActionResult<Produto> Put(int id, [FromBody] UpdateProdutoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
        {
            return BadRequest("Nome do produto é obrigatório.");
        }

        if (request.PrecoCusto < 0 || request.PrecoVenda < 0)
        {
            return BadRequest("Preços não podem ser negativos.");
        }

        var produtoAtualizado = _produtoService.Atualizar(
            id,
            request.Nome,
            request.CodigoBarras,
            request.Descricao,
            request.PrecoCusto,
            request.PrecoVenda,
            request.IdCategoria,
            request.IdFornecedor,
            request.Ativo);

        if (produtoAtualizado is null)
        {
            return NotFound();
        }

        return Ok(produtoAtualizado);
    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var removido = _produtoService.Remover(id);

        if (!removido)
        {
            return NotFound();
        }

        return NoContent();
    }
}

public sealed class CreateProdutoRequest
{
    public string Nome { get; set; } = string.Empty;
    public string? CodigoBarras { get; set; }
    public string? Descricao { get; set; }
    public decimal PrecoCusto { get; set; }
    public decimal PrecoVenda { get; set; }
    public int? IdCategoria { get; set; }
    public int? IdFornecedor { get; set; }
    public bool Ativo { get; set; } = true;
}

public sealed class UpdateProdutoRequest
{
    public string Nome { get; set; } = string.Empty;
    public string? CodigoBarras { get; set; }
    public string? Descricao { get; set; }
    public decimal PrecoCusto { get; set; }
    public decimal PrecoVenda { get; set; }
    public int? IdCategoria { get; set; }
    public int? IdFornecedor { get; set; }
    public bool Ativo { get; set; } = true;
}