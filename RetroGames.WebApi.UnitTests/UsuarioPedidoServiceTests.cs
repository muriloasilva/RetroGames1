using Microsoft.AspNetCore.Identity;
using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;

namespace RetroGames.WebApi.UnitTests;

public class UsuarioServiceTests
{
    [Fact]
    public async Task Cadastrar_EmailDuplicado_LancaEmailJaCadastrado()
    {
        var repositorioSet = new Mock<IRepositorySet>();
        var usuarioRepository = new Mock<IUsuarioRepository>();
        var passwordHasher = new Mock<IPasswordHasher<Usuario>>();
        repositorioSet.SetupGet(x => x.Usuario).Returns(usuarioRepository.Object);
        usuarioRepository.Setup(x => x.ExisteComEmailAsync("cliente@teste.com")).ReturnsAsync(true);

        var service = new UsuarioService(repositorioSet.Object, passwordHasher.Object);

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => service.CadastrarAsync(
            new Usuario { Nome = "Cliente", Email = "cliente@teste.com" }, "senha123"));

        Assert.Equal(BusinessErrors.EmailJaCadastrado, erro.Erro);
    }

    [Fact]
    public async Task Cadastrar_DadosNovos_CriaUsuarioClienteComHash()
    {
        var repositorioSet = new Mock<IRepositorySet>();
        var usuarioRepository = new Mock<IUsuarioRepository>();
        var passwordHasher = new Mock<IPasswordHasher<Usuario>>();
        repositorioSet.SetupGet(x => x.Usuario).Returns(usuarioRepository.Object);
        usuarioRepository.Setup(x => x.ExisteComEmailAsync("cliente@teste.com")).ReturnsAsync(false);
        passwordHasher.Setup(x => x.HashPassword(It.IsAny<Usuario>(), "senha123")).Returns("hash-teste");
        usuarioRepository.Setup(x => x.InserirAsync(It.IsAny<Usuario>())).ReturnsAsync(20);

        var service = new UsuarioService(repositorioSet.Object, passwordHasher.Object);
        var usuario = await service.CadastrarAsync(
            new Usuario { Nome = " Cliente ", Email = "Cliente@Teste.com" }, "senha123");

        Assert.Equal(20, usuario.Id);
        Assert.Equal("Cliente", usuario.Nome);
        Assert.Equal("cliente@teste.com", usuario.Email);
        Assert.Equal("hash-teste", usuario.SenhaHash);
        Assert.Equal(2, usuario.IdPerfil);
    }
}

public class PedidoServiceTests
{
    private readonly Mock<IRepositorySet> _repositorioSet = new();
    private readonly Mock<IJogoRepository> _jogoRepository = new();
    private readonly Mock<IPedidoRepository> _pedidoRepository = new();
    private readonly Mock<IItemPedidoRepository> _itemPedidoRepository = new();
    private readonly Mock<ISituacaoPedidoRepository> _situacaoRepository = new();

    public PedidoServiceTests()
    {
        _repositorioSet.SetupGet(x => x.Jogo).Returns(_jogoRepository.Object);
        _repositorioSet.SetupGet(x => x.Pedido).Returns(_pedidoRepository.Object);
        _repositorioSet.SetupGet(x => x.ItemPedido).Returns(_itemPedidoRepository.Object);
        _repositorioSet.SetupGet(x => x.SituacaoPedido).Returns(_situacaoRepository.Object);
    }

    [Fact]
    public async Task Criar_EstoqueInsuficiente_LancaEstoqueInsuficiente()
    {
        _jogoRepository.Setup(x => x.BuscarPorIdAsync(18)).ReturnsAsync(new Jogo
        {
            Id = 18,
            Nome = "Tetris",
            Preco = 95,
            Estoque = 1,
            Ativo = true
        });

        var service = new PedidoService(_repositorioSet.Object);

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            service.CriarAsync(20, new Dictionary<int, int> { [18] = 2 }));

        Assert.Equal(BusinessErrors.EstoqueInsuficiente, erro.Erro);
    }

    [Fact]
    public async Task Criar_JogoInativo_LancaJogoIndisponivel()
    {
        _jogoRepository.Setup(x => x.BuscarPorIdAsync(20)).ReturnsAsync(new Jogo
        {
            Id = 20,
            Nome = "River Raid",
            Preco = 85,
            Estoque = 1,
            Ativo = false
        });

        var service = new PedidoService(_repositorioSet.Object);

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            service.CriarAsync(20, new Dictionary<int, int> { [20] = 1 }));

        Assert.Equal(BusinessErrors.JogoIndisponivel, erro.Erro);
    }

    [Fact]
    public async Task Criar_ItemValido_CalculaTotalECriaPedido()
    {
        _jogoRepository.Setup(x => x.BuscarPorIdAsync(18)).ReturnsAsync(new Jogo
        {
            Id = 18,
            Nome = "Tetris",
            Preco = 95,
            Estoque = 7,
            Ativo = true
        });
        _pedidoRepository.Setup(x => x.InserirAsync(It.Is<Pedido>(p => p.Total == 190 && p.IdUsuario == 20)))
            .ReturnsAsync(42);
        _situacaoRepository.Setup(x => x.BuscarPorIdAsync(1)).ReturnsAsync(new SituacaoPedido
        {
            Id = 1,
            Descricao = "Pendente"
        });
        _itemPedidoRepository.Setup(x => x.ListarPorPedidoAsync(42)).ReturnsAsync(new List<ItemPedido>
        {
            new() { Id = 5, IdPedido = 42, IdJogo = 18, Quantidade = 2, PrecoUnitario = 95 }
        });

        var service = new PedidoService(_repositorioSet.Object);
        var resultado = await service.CriarAsync(20, new Dictionary<int, int> { [18] = 2 });

        Assert.Equal(190, resultado.Pedido.Total);
        Assert.Equal("Pendente", resultado.Situacao);
        Assert.Single(resultado.Itens);
        _itemPedidoRepository.Verify(x => x.InserirAsync(It.Is<ItemPedido>(i => i.Quantidade == 2)), Times.Once);
        _jogoRepository.Verify(x => x.AlterarEstoqueAsync(18, 5), Times.Once);
    }
}
