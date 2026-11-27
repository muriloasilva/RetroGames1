namespace RetroGames.Services;

public interface IPedidoService
{
    Task<PedidoDetalheServiceModel> CriarAsync(int idUsuario, Dictionary<int, int> itens);
    Task<PedidoDetalheServiceModel> BuscarPorIdAsync(int idUsuario, int idPedido);
    Task<List<PedidoDetalheServiceModel>> ListarPorUsuarioAsync(int idUsuario);
}
