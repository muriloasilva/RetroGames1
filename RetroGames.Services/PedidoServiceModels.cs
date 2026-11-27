using RetroGames.Model.Model;

namespace RetroGames.Services;

public class ItemPedidoDetalheServiceModel
{
    public int Id { get; set; }
    public int IdJogo { get; set; }
    public string NomeJogo { get; set; } = string.Empty;
    public decimal PrecoUnitario { get; set; }
    public int Quantidade { get; set; }
    public decimal Subtotal { get; set; }
}

public class PedidoDetalheServiceModel
{
    public Pedido Pedido { get; set; } = new();
    public string Situacao { get; set; } = string.Empty;
    public List<ItemPedidoDetalheServiceModel> Itens { get; set; } = new();
}
