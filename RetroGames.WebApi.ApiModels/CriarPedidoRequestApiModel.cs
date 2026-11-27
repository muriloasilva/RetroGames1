namespace RetroGames.WebApi.ApiModels;

public class CriarPedidoRequestApiModel
{
    public Dictionary<int, int> Itens { get; set; } = new();
}
