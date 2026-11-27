using MySqlConnector;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class SituacaoPedidoRepository : RepositoryBase, ISituacaoPedidoRepository
{
    public SituacaoPedidoRepository(RepositoryConfig config) : base(config) { }

    public async Task<List<SituacaoPedido>> ListarAsync()
    {
        const string sql = @"
            SELECT co_situacao_pedido, ds_situacao_pedido
            FROM tb_situacao_pedido
            ORDER BY co_situacao_pedido;";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        await using var leitor = await comando.ExecuteReaderAsync();

        var situacoes = new List<SituacaoPedido>();
        while (await leitor.ReadAsync())
        {
            situacoes.Add(new SituacaoPedido
            {
                Id = leitor.GetInt32("co_situacao_pedido"),
                Descricao = leitor.GetString("ds_situacao_pedido")
            });
        }
        return situacoes;
    }

    public async Task<SituacaoPedido?> BuscarPorIdAsync(int id)
    {
        const string sql = @"
            SELECT co_situacao_pedido, ds_situacao_pedido
            FROM tb_situacao_pedido
            WHERE co_situacao_pedido = @id;";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@id", id);
        await using var leitor = await comando.ExecuteReaderAsync();

        if (await leitor.ReadAsync())
        {
            return new SituacaoPedido
            {
                Id = leitor.GetInt32("co_situacao_pedido"),
                Descricao = leitor.GetString("ds_situacao_pedido")
            };
        }
        return null;
    }
}
