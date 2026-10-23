using Microsoft.AspNetCore.Mvc;
using RecebimentoMercadorias.Services;

namespace RecebimentoMercadorias.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class FornecedoresController : ControllerBase
{
    private readonly FornecedorService _fornecedorService;

    public FornecedoresController(FornecedorService fornecedorService)
    {
        _fornecedorService = fornecedorService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Fornecedor>> Get()
    {
        return Ok(_fornecedorService.ObterTodos());
    }

    [HttpGet("{id:int}")]
    public ActionResult<Fornecedor> GetById(int id)
    {
        var fornecedor = _fornecedorService.ObterPorId(id);

        if (fornecedor is null)
        {
            return NotFound();
        }

        return Ok(fornecedor);
    }

    [HttpPost]
    public ActionResult<Fornecedor> Post([FromBody] CreateFornecedorRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome) ||
            string.IsNullOrWhiteSpace(request.Cnpj) ||
            string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest("Nome, CNPJ e e-mail são obrigatórios.");
        }

        var fornecedor = _fornecedorService.Criar(
            request.Nome,
            request.Cnpj,
            request.Email,
            request.Telefone,
            request.Ativo);

        return CreatedAtAction(nameof(GetById), new { id = fornecedor.Id }, fornecedor);
    }

    [HttpPut("{id:int}")]
    public ActionResult<Fornecedor> Put(int id, [FromBody] UpdateFornecedorRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome) ||
            string.IsNullOrWhiteSpace(request.Cnpj) ||
            string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest("Nome, CNPJ e e-mail são obrigatórios.");
        }

        var fornecedorAtualizado = _fornecedorService.Atualizar(
            id,
            request.Nome,
            request.Cnpj,
            request.Email,
            request.Telefone,
            request.Ativo);

        if (fornecedorAtualizado is null)
        {
            return NotFound();
        }

        return Ok(fornecedorAtualizado);
    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var removido = _fornecedorService.Remover(id);

        if (!removido)
        {
            return NotFound();
        }

        return NoContent();
    }
}

public sealed class CreateFornecedorRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
}

public sealed class UpdateFornecedorRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
}
