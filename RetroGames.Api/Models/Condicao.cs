namespace RetroGames.Api.Models;

public class Condicao
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty; // ex: Lacrado, Na Caixa, Completo, Apenas Cartucho

    public ICollection<Jogo> Jogos { get; set; } = new List<Jogo>();
}