using Microsoft.EntityFrameworkCore;
using RetroGames.Api.Models;

namespace RetroGames.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Jogo> Jogos { get; set; }
    public DbSet<Plataforma> Plataformas { get; set; }
    public DbSet<Genero> Generos { get; set; }
    public DbSet<Condicao> Condicoes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração de precisão decimal para o Preço
        modelBuilder.Entity<Jogo>()
            .Property(j => j.Preco)
            .HasPrecision(10, 2);
    }
}