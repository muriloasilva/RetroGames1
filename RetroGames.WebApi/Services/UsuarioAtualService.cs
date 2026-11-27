using System.Security.Claims;
using RetroGames.Services;

namespace RetroGames.WebApi.Services;

public class UsuarioAtualService : IUsuarioAtualService
{
    private readonly ClaimsPrincipal _usuario;

    public UsuarioAtualService(IHttpContextAccessor httpContextAccessor)
    {
        _usuario = httpContextAccessor.HttpContext?.User ?? new ClaimsPrincipal();
    }

    public bool EstaLogado => _usuario.Identity?.IsAuthenticated == true;

    public int? Id
    {
        get
        {
            var valor = _usuario.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(valor, out var id) ? id : null;
        }
    }

    public string Nome => _usuario.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
    public string Email => _usuario.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
    public string Perfil => _usuario.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
    public bool EAdministrador => Perfil == "Administrador" || _usuario.IsInRole("Administrador");
}
