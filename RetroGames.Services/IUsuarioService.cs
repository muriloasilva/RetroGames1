using RetroGames.Model.Model;

namespace RetroGames.Services;

public interface IUsuarioService
{
    Task<Usuario> CadastrarAsync(Usuario usuario, string senha);
}
