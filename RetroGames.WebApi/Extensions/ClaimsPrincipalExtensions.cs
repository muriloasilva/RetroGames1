using System.Security.Claims;

namespace RetroGames.WebApi.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int? ObterUsuarioId(this ClaimsPrincipal usuario)
    {
        var valor = usuario.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(valor, out var id) ? id : null;
    }

    public static string ObterUsuarioNome(this ClaimsPrincipal usuario)
    {
        return usuario.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
    }

    public static string ObterUsuarioEmail(this ClaimsPrincipal usuario)
    {
        return usuario.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
    }

    public static string ObterUsuarioPerfil(this ClaimsPrincipal usuario)
    {
        return usuario.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
    }
}
