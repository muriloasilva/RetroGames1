namespace RetroGames.Api.Models;

public class Jogo
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal Preco { get; set; }
    public int Ano { get; set; }
    public string? ImagemUrl { get; set; }

    // Chaves Estrangeiras (Foreign Keys)
    public int PlataformaId { get; set; }
    public Plataforma? Plataforma { get; set; }

    public int GeneroId { get; set; }
    public Genero? Genero { get; set; }

    public int CondicaoId { get; set; }
    public Condicao? Condicao { get; set; }
}