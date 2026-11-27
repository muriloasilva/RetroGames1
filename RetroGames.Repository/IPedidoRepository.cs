using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface IPedidoRepository
{
    Task<Pedido?> BuscarPorIdAsync(int id);
    Task<List<Pedido>> ListarPorUsuarioAsync(int idUsuario);
    Task<int> InserirAsync(Pedido pedido);
}
