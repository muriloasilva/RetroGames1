using MySqlConnector;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class PedidoRepository : RepositoryBase, IPedidoRepository
{
    public PedidoRepository(RepositoryConfig config) : base(config) { }

    public async Task<Pedido?> BuscarPorIdAsync(int id)
    {
        const string sql = @"
            SELECT co_seq_pedido, dt_pedido, vl_total, co_usuario, co_situacao_pedido
            FROM tb_pedido
            WHERE co_seq_pedido = @id;";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@id", id);
        await using var leitor = await comando.ExecuteReaderAsync();

        if (await leitor.ReadAsync()) return Mapear(leitor);
        return null;
    }

    public async Task<List<Pedido>> ListarPorUsuarioAsync(int idUsuario)
    {
        const string sql = @"
            SELECT co_seq_pedido, dt_pedido, vl_total, co_usuario, co_situacao_pedido
            FROM tb_pedido
            WHERE co_usuario = @idUsuario
            ORDER BY dt_pedido DESC, co_seq_pedido DESC;";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@idUsuario", idUsuario);
        await using var leitor = await comando.ExecuteReaderAsync();

        var pedidos = new List<Pedido>();
        while (await leitor.ReadAsync()) pedidos.Add(Mapear(leitor));
        return pedidos;
    }

    public async Task<int> InserirAsync(Pedido pedido)
    {
        const string sql = @"
            INSERT INTO tb_pedido (dt_pedido, vl_total, co_usuario, co_situacao_pedido)
            VALUES (@dataPedido, @total, @idUsuario, @idSituacaoPedido);";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@dataPedido", pedido.DataPedido);
        AdicionarParametro(comando, "@total", pedido.Total);
        AdicionarParametro(comando, "@idUsuario", pedido.IdUsuario);
        AdicionarParametro(comando, "@idSituacaoPedido", pedido.IdSituacaoPedido);

        return (int)await ExecutarGravacaoAsync(comando);
    }

    private static Pedido Mapear(MySqlDataReader leitor)
    {
        return new Pedido
        {
            Id = leitor.GetInt32("co_seq_pedido"),
            DataPedido = leitor.GetDateTime("dt_pedido"),
            Total = leitor.GetDecimal("vl_total"),
            IdUsuario = leitor.GetInt32("co_usuario"),
            IdSituacaoPedido = leitor.GetInt32("co_situacao_pedido")
        };
    }
}
