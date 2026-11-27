using RetroGames.Model.Model;

namespace RetroGames.Services;

public interface IJogoService
{
    Task<(List<JogoDetalheServiceModel> Jogos, int Total)> ListarAsync(
        string? busca = null,
        int? idPlataforma = null,
        int? idGenero = null,
        bool incluirInativos = false,
        int pagina = 1,
        int tamanhoPagina = 10);

    Task<JogoDetalheServiceModel> BuscarPorIdAsync(int id, bool incluirInativos = false);
    Task<JogoDetalheServiceModel> InserirAsync(Jogo jogo, List<int> idsGeneros);
    Task AlterarAsync(Jogo jogo, List<int> idsGeneros);
    Task DesativarAsync(int id);
}
