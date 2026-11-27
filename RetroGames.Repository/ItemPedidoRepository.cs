using MySqlConnector;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class ItemPedidoRepository : RepositoryBase, IItemPedidoRepository
{
    public ItemPedidoRepository(RepositoryConfig config) : base(config) { }

    public async Task<List<ItemPedido>> ListarPorPedidoAsync(int idPedido)
    {
        const string sql = @"
            SELECT co_seq_item_pedido, vl_unitario, qt_item, co_pedido, co_jogo
            FROM tb_item_pedido
            WHERE co_pedido = @idPedido
            ORDER BY co_seq_item_pedido;";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@idPedido", idPedido);
        await using var leitor = await comando.ExecuteReaderAsync();

        var itens = new List<ItemPedido>();
        while (await leitor.ReadAsync())
        {
            itens.Add(new ItemPedido
            {
                Id = leitor.GetInt32("co_seq_item_pedido"),
                PrecoUnitario = leitor.GetDecimal("vl_unitario"),
                Quantidade = leitor.GetInt32("qt_item"),
                IdPedido = leitor.GetInt32("co_pedido"),
                IdJogo = leitor.GetInt32("co_jogo")
            });
        }
        return itens;
    }

    public async Task InserirAsync(ItemPedido item)
    {
        const string sql = @"
            INSERT INTO tb_item_pedido (vl_unitario, qt_item, co_pedido, co_jogo)
            VALUES (@precoUnitario, @quantidade, @idPedido, @idJogo);";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@precoUnitario", item.PrecoUnitario);
        AdicionarParametro(comando, "@quantidade", item.Quantidade);
        AdicionarParametro(comando, "@idPedido", item.IdPedido);
        AdicionarParametro(comando, "@idJogo", item.IdJogo);
        await ExecutarGravacaoAsync(comando);
    }
}
