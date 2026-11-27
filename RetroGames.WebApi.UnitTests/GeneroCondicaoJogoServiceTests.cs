using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;

namespace RetroGames.WebApi.UnitTests;

public class GeneroCondicaoJogoServiceTests
{
    [Fact]
    public async Task GeneroService_Listar_DevolveDadosDoRepository()
    {
        var repositorySet = new Mock<IRepositorySet>();
        var generoRepository = new Mock<IGeneroRepository>();
        var generos = new List<Genero>
        {
            new() { Id = 1, Nome = "Plataforma" },
            new() { Id = 2, Nome = "RPG" }
        };

        repositorySet.SetupGet(x => x.Genero).Returns(generoRepository.Object);
        generoRepository.Setup(x => x.ListarAsync()).ReturnsAsync(generos);

        var service = new GeneroService(repositorySet.Object);
        var resultado = await service.ListarAsync();

        Assert.Equal(generos, resultado);
        generoRepository.Verify(x => x.ListarAsync(), Times.Once);
    }

    [Fact]
    public async Task CondicaoJogoService_Listar_DevolveDadosDoRepository()
    {
        var repositorySet = new Mock<IRepositorySet>();
        var condicaoRepository = new Mock<ICondicaoJogoRepository>();
        var condicoes = new List<CondicaoJogo>
        {
            new() { Id = 1, Descricao = "Novo" },
            new() { Id = 2, Descricao = "Usado" }
        };

        repositorySet.SetupGet(x => x.CondicaoJogo).Returns(condicaoRepository.Object);
        condicaoRepository.Setup(x => x.ListarAsync()).ReturnsAsync(condicoes);

        var service = new CondicaoJogoService(repositorySet.Object);
        var resultado = await service.ListarAsync();

        Assert.Equal(condicoes, resultado);
        condicaoRepository.Verify(x => x.ListarAsync(), Times.Once);
    }
}
