namespace RetroGames.Api.Models;

public class Plataforma
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Fabricante { get; set; } = string.Empty;
    public int? AnoLancamento { get; set; }
    
    public ICollection<Jogo> Jogos { get; set; } = new List<Jogo>();
}