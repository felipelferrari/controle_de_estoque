using Microsoft.AspNetCore.Mvc;
using RecebimentoMercadorias.Entities;
using RecebimentoMercadorias.Services;

namespace RecebimentoMercadorias.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CategoriasController : ControllerBase
{
    private readonly CategoriaService _categoriaService;

    public CategoriasController(CategoriaService categoriaService)
    {
        _categoriaService = categoriaService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Categoria>> Get()
    {
        return Ok(_categoriaService.ObterTodos());
    }

    [HttpGet("{id:int}")]
    public ActionResult<Categoria> GetById(int id)
    {
        var categoria = _categoriaService.ObterPorId(id);
        return categoria is null ? NotFound() : Ok(categoria);
    }

    [HttpPost]
    public ActionResult<Categoria> Post([FromBody] CategoriaRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
        {
            return BadRequest("Nome da categoria é obrigatório.");
        }

        var categoria = _categoriaService.Criar(request.Nome, request.Descricao, request.Ativo);
        return CreatedAtAction(nameof(GetById), new { id = categoria.Id }, categoria);
    }

    [HttpPut("{id:int}")]
    public ActionResult<Categoria> Put(int id, [FromBody] CategoriaRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
        {
            return BadRequest("Nome da categoria é obrigatório.");
        }

        var categoria = _categoriaService.Atualizar(id, request.Nome, request.Descricao, request.Ativo);
        return categoria is null ? NotFound() : Ok(categoria);
    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        try
        {
            return _categoriaService.Remover(id) ? NoContent() : NotFound();
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(exception.Message);
        }
    }
}

public sealed class CategoriaRequest
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public bool Ativo { get; set; } = true;
}