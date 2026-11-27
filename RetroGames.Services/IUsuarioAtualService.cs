namespace RetroGames.Services;

public interface IUsuarioAtualService
{
    bool EstaLogado { get; }
    int? Id { get; }
    string Nome { get; }
    string Email { get; }
    string Perfil { get; }
    bool EAdministrador { get; }
}
