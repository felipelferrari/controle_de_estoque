using Microsoft.AspNetCore.Mvc;
using RecebimentoMercadorias.Entities;
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
        if (string.IsNullOrWhiteSpace(request.RazaoSocial) ||
            string.IsNullOrWhiteSpace(request.Cnpj))
        {
            return BadRequest("Razão social e CNPJ são obrigatórios.");
        }

        var fornecedor = _fornecedorService.Criar(
            request.RazaoSocial,
            request.NomeFantasia,
            request.Cnpj,
            request.Telefone,
            request.Email,
            request.Endereco,
            request.Ativo);

        return CreatedAtAction(nameof(GetById), new { id = fornecedor.Id }, fornecedor);
    }

    [HttpPut("{id:int}")]
    public ActionResult<Fornecedor> Put(int id, [FromBody] UpdateFornecedorRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RazaoSocial) ||
            string.IsNullOrWhiteSpace(request.Cnpj))
        {
            return BadRequest("Razão social e CNPJ são obrigatórios.");
        }

        var fornecedorAtualizado = _fornecedorService.Atualizar(
            id,
            request.RazaoSocial,
            request.NomeFantasia,
            request.Cnpj,
            request.Telefone,
            request.Email,
            request.Endereco,
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
    public string RazaoSocial { get; set; } = string.Empty;
    public string? NomeFantasia { get; set; }
    public string Cnpj { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public string? Endereco { get; set; }
    public bool Ativo { get; set; } = true;
}

public sealed class UpdateFornecedorRequest
{
    public string RazaoSocial { get; set; } = string.Empty;
    public string? NomeFantasia { get; set; }
    public string Cnpj { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public string? Endereco { get; set; }
    public bool Ativo { get; set; } = true;
}