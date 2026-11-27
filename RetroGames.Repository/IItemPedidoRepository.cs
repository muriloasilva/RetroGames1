using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface IItemPedidoRepository
{
    Task<List<ItemPedido>> ListarPorPedidoAsync(int idPedido);
    Task InserirAsync(ItemPedido item);
}
