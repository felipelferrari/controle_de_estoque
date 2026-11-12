using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using RecebimentoMercadorias.Data;

namespace RecebimentoMercadorias.Security;

public sealed class StockUserClaimsTransformation : IClaimsTransformation
{
    private readonly AppDbContext _context;

    public StockUserClaimsTransformation(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity is not ClaimsIdentity identity ||
            !identity.IsAuthenticated ||
            identity.HasClaim("stock_access", "true") ||
            !Guid.TryParse(principal.FindFirstValue("sub"), out var authUserId))
        {
            return principal;
        }

        var usuario = await _context.Usuarios
            .AsNoTracking()
            .SingleOrDefaultAsync(user => user.AuthUserId == authUserId && user.Ativo);

        if (usuario is null)
        {
            return principal;
        }

        identity.AddClaim(new Claim("stock_access", "true"));
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()));
        identity.AddClaim(new Claim(ClaimTypes.Name, usuario.Nome));
        identity.AddClaim(new Claim(ClaimTypes.Role, usuario.Cargo.Trim().ToUpperInvariant()));

        return principal;
    }
}
