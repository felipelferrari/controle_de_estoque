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
        try
        {
            var fornecedores = _fornecedorService.ObterTodos() ?? new();
            var usuarios = _usuarioService.ObterTodos() ?? new();

            var response = new ResumoEstoqueResponse
            {
                TotalFornecedores = fornecedores.Count,
                FornecedoresAtivos = fornecedores.Count(f => f.Ativo),
                TotalUsuarios = usuarios.Count,
                UsuariosAtivos = usuarios.Count(u => u.Ativo),
                UltimoFornecedorCadastrado = fornecedores
                    .OrderByDescending(f => f.DataCadastro)
                    .Select(f => !string.IsNullOrWhiteSpace(f.NomeFantasia) ? f.NomeFantasia : f.RazaoSocial)
                    .FirstOrDefault() ?? "Nenhum fornecedor cadastrado",
                UltimoUsuarioCadastrado = usuarios
                    .OrderByDescending(u => u.DataCadastro)
                    .Select(u => u.Nome)
                    .FirstOrDefault() ?? "Nenhum usuário cadastrado"
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Erro ao gerar resumo", Error = ex.Message });
        }
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