using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;

namespace RetroGames.WebApi.UnitTests;

public class PlataformaServiceTests
{
    private readonly Mock<IRepositorySet> _repositorioSet;
    private readonly Mock<IPlataformaRepository> _plataformaRepository;
    private readonly PlataformaService _service;

    public PlataformaServiceTests()
    {
        _repositorioSet = new Mock<IRepositorySet>();
        _plataformaRepository = new Mock<IPlataformaRepository>();
        _repositorioSet.SetupGet(x => x.Plataforma).Returns(_plataformaRepository.Object);
        _service = new PlataformaService(_repositorioSet.Object);
    }

    [Fact]
    public async Task Inserir_ComNomeDuplicado_LancaPlataformaNomeDuplicado()
    {
        var plataforma = new Plataforma { Nome = " Super Nintendo " };
        _plataformaRepository
            .Setup(x => x.ExisteComNomeAsync("Super Nintendo", null))
            .ReturnsAsync(true);

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => _service.InserirAsync(plataforma));

        Assert.Equal(BusinessErrors.PlataformaNomeDuplicado, erro.Erro);
    }

    [Fact]
    public async Task Inserir_QuandoRepositoryLancaDuplicado_LancaPlataformaNomeDuplicado()
    {
        var plataforma = new Plataforma { Nome = "Mega Drive" };
        _plataformaRepository.Setup(x => x.ExisteComNomeAsync("Mega Drive", null)).ReturnsAsync(false);
        _plataformaRepository.Setup(x => x.InserirAsync(plataforma))
            .ThrowsAsync(new RegistroDuplicadoException("Duplicado"));

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => _service.InserirAsync(plataforma));

        Assert.Equal(BusinessErrors.PlataformaNomeDuplicado, erro.Erro);
    }

    [Fact]
    public async Task Inserir_RegistroLivre_DevolvePlataformaComId()
    {
        var plataforma = new Plataforma { Nome = " PlayStation " };
        _plataformaRepository.Setup(x => x.ExisteComNomeAsync("PlayStation", null)).ReturnsAsync(false);
        _plataformaRepository.Setup(x => x.InserirAsync(plataforma)).ReturnsAsync(12);

        var criada = await _service.InserirAsync(plataforma);

        Assert.Equal(12, criada.Id);
        Assert.Equal("PlayStation", criada.Nome);
    }

    [Fact]
    public async Task Alterar_MantendoProprioNome_GravaAlteracao()
    {
        var plataforma = new Plataforma { Id = 1, Nome = " Super Nintendo " };
        _plataformaRepository.Setup(x => x.BuscarPorIdAsync(1))
            .ReturnsAsync(new Plataforma { Id = 1, Nome = "Super Nintendo" });
        _plataformaRepository.Setup(x => x.ExisteComNomeAsync("Super Nintendo", 1)).ReturnsAsync(false);

        await _service.AlterarAsync(plataforma);

        _plataformaRepository.Verify(x => x.AlterarAsync(plataforma), Times.Once);
    }

    [Fact]
    public async Task Excluir_PlataformaComJogos_LancaPlataformaPossuiJogos()
    {
        _plataformaRepository.Setup(x => x.BuscarPorIdAsync(1))
            .ReturnsAsync(new Plataforma { Id = 1, Nome = "Super Nintendo" });
        _plataformaRepository.Setup(x => x.PossuiJogosAsync(1)).ReturnsAsync(true);

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => _service.ExcluirAsync(1));

        Assert.Equal(BusinessErrors.PlataformaPossuiJogos, erro.Erro);
        _plataformaRepository.Verify(x => x.ExcluirAsync(It.IsAny<int>()), Times.Never);
    }
}
