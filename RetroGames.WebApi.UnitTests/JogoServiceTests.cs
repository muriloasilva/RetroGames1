using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;

namespace RetroGames.WebApi.UnitTests;

public class JogoServiceTests
{
    private readonly Mock<IRepositorySet> _repositorioSet = new();
    private readonly Mock<IJogoRepository> _jogoRepository = new();
    private readonly Mock<IJogoGeneroRepository> _jogoGeneroRepository = new();
    private readonly Mock<IPlataformaRepository> _plataformaRepository = new();
    private readonly Mock<ICondicaoJogoRepository> _condicaoRepository = new();
    private readonly Mock<IGeneroRepository> _generoRepository = new();
    private readonly JogoService _service;

    public JogoServiceTests()
    {
        _repositorioSet.SetupGet(x => x.Jogo).Returns(_jogoRepository.Object);
        _repositorioSet.SetupGet(x => x.JogoGenero).Returns(_jogoGeneroRepository.Object);
        _repositorioSet.SetupGet(x => x.Plataforma).Returns(_plataformaRepository.Object);
        _repositorioSet.SetupGet(x => x.CondicaoJogo).Returns(_condicaoRepository.Object);
        _repositorioSet.SetupGet(x => x.Genero).Returns(_generoRepository.Object);

        _plataformaRepository.Setup(x => x.ListarAsync()).ReturnsAsync(new List<Plataforma>
        {
            new() { Id = 1, Nome = "Super Nintendo" }
        });
        _condicaoRepository.Setup(x => x.ListarAsync()).ReturnsAsync(new List<CondicaoJogo>
        {
            new() { Id = 1, Descricao = "Novo" }
        });
        _generoRepository.Setup(x => x.ListarAsync()).ReturnsAsync(new List<Genero>
        {
            new() { Id = 1, Nome = "Plataforma" }
        });

        _service = new JogoService(_repositorioSet.Object);
    }

    [Fact]
    public async Task Inserir_ComPlataformaInexistente_LancaPlataformaNaoEncontrada()
    {
        _plataformaRepository.Setup(x => x.BuscarPorIdAsync(99)).ReturnsAsync((Plataforma?)null);

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            _service.InserirAsync(new Jogo { IdPlataforma = 99, IdCondicaoJogo = 1 }, new List<int> { 1 }));

        Assert.Equal(BusinessErrors.PlataformaNaoEncontrada, erro.Erro);
    }

    [Fact]
    public async Task Inserir_ComCondicaoInexistente_LancaCondicaoNaoEncontrada()
    {
        _plataformaRepository.Setup(x => x.BuscarPorIdAsync(1)).ReturnsAsync(new Plataforma { Id = 1 });

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            _service.InserirAsync(new Jogo { IdPlataforma = 1, IdCondicaoJogo = 99 }, new List<int> { 1 }));

        Assert.Equal(BusinessErrors.CondicaoNaoEncontrada, erro.Erro);
    }

    [Fact]
    public async Task Inserir_ComGeneroInexistente_LancaGeneroNaoEncontrado()
    {
        _plataformaRepository.Setup(x => x.BuscarPorIdAsync(1)).ReturnsAsync(new Plataforma { Id = 1 });

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            _service.InserirAsync(new Jogo { IdPlataforma = 1, IdCondicaoJogo = 1 }, new List<int> { 99 }));

        Assert.Equal(BusinessErrors.GeneroNaoEncontrado, erro.Erro);
    }

    [Fact]
    public async Task BuscarPorId_Inexistente_LancaJogoNaoEncontrado()
    {
        _jogoRepository.Setup(x => x.BuscarPorIdAsync(99)).ReturnsAsync((Jogo?)null);

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => _service.BuscarPorIdAsync(99));

        Assert.Equal(BusinessErrors.JogoNaoEncontrado, erro.Erro);
    }

    [Fact]
    public async Task Desativar_Inexistente_LancaJogoNaoEncontrado()
    {
        _jogoRepository.Setup(x => x.BuscarPorIdAsync(99)).ReturnsAsync((Jogo?)null);

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => _service.DesativarAsync(99));

        Assert.Equal(BusinessErrors.JogoNaoEncontrado, erro.Erro);
    }

    [Fact]
    public async Task Listar_FiltrandoPorGenero_ConsultaSomenteIdsVinculados()
    {
        _jogoGeneroRepository.Setup(x => x.ListarTodosAsync()).ReturnsAsync(new List<JogoGenero>
        {
            new() { IdJogo = 7, IdGenero = 1 },
            new() { IdJogo = 8, IdGenero = 2 }
        });
        _jogoRepository
            .Setup(x => x.ListarAsync(null, null, It.Is<List<int>>(ids => ids.SequenceEqual(new[] { 7 })), false, 1, 12))
            .ReturnsAsync((new List<Jogo>(), 0));

        await _service.ListarAsync(idGenero: 1, pagina: 1, tamanhoPagina: 12);

        _jogoRepository.Verify(x => x.ListarAsync(
            null, null, It.Is<List<int>>(ids => ids.SequenceEqual(new[] { 7 })), false, 1, 12), Times.Once);
    }

    [Fact]
    public async Task Listar_GeneroSemJogos_DevolveResultadoVazioSemDetalhes()
    {
        _jogoGeneroRepository.Setup(x => x.ListarTodosAsync()).ReturnsAsync(new List<JogoGenero>());
        _jogoRepository
            .Setup(x => x.ListarAsync(null, null, It.Is<List<int>>(ids => ids.Count == 0), false, 1, 10))
            .ReturnsAsync((new List<Jogo>(), 0));

        var resultado = await _service.ListarAsync(idGenero: 1);

        Assert.Empty(resultado.Jogos);
        Assert.Equal(0, resultado.Total);
        _jogoRepository.Verify(x => x.ListarAsync(
            null, null, It.Is<List<int>>(ids => ids.Count == 0), false, 1, 10), Times.Once);
    }
}
