using System.Transactions;
using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public class PedidoService : IPedidoService
{
    private readonly IRepositorySet _repositorio;

    public PedidoService(IRepositorySet repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<PedidoDetalheServiceModel> CriarAsync(int idUsuario, Dictionary<int, int> itens)
    {
        if (itens.Count == 0)
        {
            throw new BusinessRuleException(BusinessErrors.EstoqueInsuficiente);
        }

        var itensValidados = new Dictionary<int, (Jogo Jogo, int Quantidade)>();
        foreach (var (idJogo, quantidade) in itens)
        {
            if (quantidade <= 0)
            {
                throw new BusinessRuleException(BusinessErrors.EstoqueInsuficiente);
            }

            var jogo = await _repositorio.Jogo.BuscarPorIdAsync(idJogo);
            if (jogo is null || !jogo.Ativo)
            {
                throw new BusinessRuleException(BusinessErrors.JogoIndisponivel);
            }

            if (jogo.Estoque < quantidade)
            {
                throw new BusinessRuleException(BusinessErrors.EstoqueInsuficiente);
            }

            itensValidados[idJogo] = (jogo, quantidade);
        }

        var total = itensValidados.Values.Sum(item => item.Jogo.Preco * item.Quantidade);
        var pedido = new Pedido
        {
            DataPedido = DateTime.UtcNow,
            Total = total,
            IdUsuario = idUsuario,
            IdSituacaoPedido = 1
        };

        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout,
        };

        using (var transactionScope = new TransactionScope(TransactionScopeOption.Required,
                   transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
        {
            pedido.Id = await _repositorio.Pedido.InserirAsync(pedido);

            foreach (var (idJogo, item) in itensValidados)
            {
                await _repositorio.ItemPedido.InserirAsync(new ItemPedido
                {
                    PrecoUnitario = item.Jogo.Preco,
                    Quantidade = item.Quantidade,
                    IdPedido = pedido.Id,
                    IdJogo = idJogo
                });

                await _repositorio.Jogo.AlterarEstoqueAsync(idJogo, item.Jogo.Estoque - item.Quantidade);
            }

            transactionScope.Complete();
        }

        return await MontarDetalheAsync(pedido);
    }

    public async Task<PedidoDetalheServiceModel> BuscarPorIdAsync(int idUsuario, int idPedido)
    {
        var pedido = await _repositorio.Pedido.BuscarPorIdAsync(idPedido);
        if (pedido is null)
        {
            throw new BusinessRuleException(BusinessErrors.PedidoNaoEncontrado);
        }

        if (pedido.IdUsuario != idUsuario)
        {
            throw new BusinessRuleException(BusinessErrors.PedidoNaoEncontrado);
        }

        return await MontarDetalheAsync(pedido);
    }

    public async Task<List<PedidoDetalheServiceModel>> ListarPorUsuarioAsync(int idUsuario)
    {
        var pedidos = await _repositorio.Pedido.ListarPorUsuarioAsync(idUsuario);
        var detalhes = new List<PedidoDetalheServiceModel>();

        foreach (var pedido in pedidos)
        {
            detalhes.Add(await MontarDetalheAsync(pedido));
        }

        return detalhes;
    }

    private async Task<PedidoDetalheServiceModel> MontarDetalheAsync(Pedido pedido)
    {
        var situacao = await _repositorio.SituacaoPedido.BuscarPorIdAsync(pedido.IdSituacaoPedido);
        var itens = await _repositorio.ItemPedido.ListarPorPedidoAsync(pedido.Id);

        var detalhes = new List<ItemPedidoDetalheServiceModel>();
        foreach (var item in itens)
        {
            var jogo = await _repositorio.Jogo.BuscarPorIdAsync(item.IdJogo);
            detalhes.Add(new ItemPedidoDetalheServiceModel
            {
                Id = item.Id,
                IdJogo = item.IdJogo,
                NomeJogo = jogo?.Nome ?? string.Empty,
                PrecoUnitario = item.PrecoUnitario,
                Quantidade = item.Quantidade,
                Subtotal = item.PrecoUnitario * item.Quantidade
            });
        }

        return new PedidoDetalheServiceModel
        {
            Pedido = pedido,
            Situacao = situacao?.Descricao ?? string.Empty,
            Itens = detalhes
        };
    }
}
