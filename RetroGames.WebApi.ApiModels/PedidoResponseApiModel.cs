namespace RetroGames.WebApi.ApiModels;

public class ItemPedidoResponseApiModel
{
    public int IdJogo { get; set; }
    public string NomeJogo { get; set; } = string.Empty;
    public decimal PrecoUnitario { get; set; }
    public int Quantidade { get; set; }
    public decimal Subtotal { get; set; }
}

public class PedidoResponseApiModel
{
    public int Id { get; set; }
    public DateTime DataPedido { get; set; }
    public decimal Total { get; set; }
    public string Situacao { get; set; } = string.Empty;
    public List<ItemPedidoResponseApiModel> Itens { get; set; } = new();
}
