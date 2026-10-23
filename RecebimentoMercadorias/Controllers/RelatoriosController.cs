using Microsoft.AspNetCore.Mvc;
using RecebimentoMercadorias.Services;

namespace RecebimentoMercadorias.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class RelatoriosController : ControllerBase
{
    private readonly FornecedorService _fornecedorService;
    private readonly UsuarioService _usuarioService;

    public RelatoriosController(FornecedorService fornecedorService, UsuarioService usuarioService)
    {
        _fornecedorService = fornecedorService;
        _usuarioService = usuarioService;
    }

    [HttpGet("resumo")]
    public ActionResult<ResumoEstoqueResponse> GetResumo()
    {
        var fornecedores = _fornecedorService.ObterTodos();
        var usuarios = _usuarioService.ObterTodos();

        var response = new ResumoEstoqueResponse
        {
            TotalFornecedores = fornecedores.Count,
            FornecedoresAtivos = fornecedores.Count(f => f.Ativo),
            TotalUsuarios = usuarios.Count,
            UsuariosAtivos = usuarios.Count(u => u.Ativo),
            UltimoFornecedorCadastrado = fornecedores
                .OrderByDescending(f => f.DataCadastro)
                .Select(f => f.Nome)
                .FirstOrDefault(),
            UltimoUsuarioCadastrado = usuarios
                .OrderByDescending(u => u.DataCadastro)
                .Select(u => u.Nome)
                .FirstOrDefault()
        };

        return Ok(response);
    }
}

public sealed class ResumoEstoqueResponse
{
    public int TotalFornecedores { get; set; }
    public int FornecedoresAtivos { get; set; }
    public int TotalUsuarios { get; set; }
    public int UsuariosAtivos { get; set; }
    public string? UltimoFornecedorCadastrado { get; set; }
    public string? UltimoUsuarioCadastrado { get; set; }
}
