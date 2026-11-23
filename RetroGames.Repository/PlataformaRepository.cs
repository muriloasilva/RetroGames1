using MySqlConnector;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class PlataformaRepository : RepositoryBase, IPlataformaRepository
{
    public PlataformaRepository(RepositoryConfig config) : base(config)
    {
    }

    public async Task<List<Plataforma>> ListarAsync()
    {
        const string sql = @"
            SELECT co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento
            FROM tb_plataforma
            ORDER BY no_plataforma;";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        await using var leitor = await comando.ExecuteReaderAsync();

        var plataformas = new List<Plataforma>();
        while (await leitor.ReadAsync())
        {
            plataformas.Add(MapearPlataforma(leitor));
        }

        return plataformas;
    }

    public async Task<Plataforma?> BuscarPorIdAsync(int id)
    {
        const string sql = @"
            SELECT co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento
            FROM tb_plataforma
            WHERE co_seq_plataforma = @id;";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@id", id);

        await using var leitor = await comando.ExecuteReaderAsync();
        if (await leitor.ReadAsync())
        {
            return MapearPlataforma(leitor);
        }

        return null;
    }

    public async Task<bool> ExisteComNomeAsync(string nome, int? idIgnorar = null)
    {
        const string sql = @"
            SELECT COUNT(1)
            FROM tb_plataforma
            WHERE no_plataforma = @nome
              AND (@idIgnorar IS NULL OR co_seq_plataforma <> @idIgnorar);";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@nome", nome);
        AdicionarParametro(comando, "@idIgnorar", idIgnorar);

        var resultado = await comando.ExecuteScalarAsync();
        return Convert.ToInt64(resultado) > 0;
    }

    public async Task<bool> PossuiJogosAsync(int idPlataforma)
    {
        const string sql = @"
            SELECT COUNT(1)
            FROM tb_jogo
            WHERE co_plataforma = @idPlataforma;";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@idPlataforma", idPlataforma);

        var resultado = await comando.ExecuteScalarAsync();
        return Convert.ToInt64(resultado) > 0;
    }

    public async Task<int> InserirAsync(Plataforma plataforma)
    {
        const string sql = @"
            INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento)
            VALUES (@nome, @fabricante, @anoLancamento);";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@nome", plataforma.Nome);
        AdicionarParametro(comando, "@fabricante", plataforma.Fabricante);
        AdicionarParametro(comando, "@anoLancamento", plataforma.AnoLancamento);

        var idGerado = await ExecutarGravacaoAsync(comando);
        return (int)idGerado;
    }

    public async Task AlterarAsync(Plataforma plataforma)
    {
        const string sql = @"
            UPDATE tb_plataforma
            SET no_plataforma = @nome,
                no_fabricante = @fabricante,
                nu_ano_lancamento = @anoLancamento
            WHERE co_seq_plataforma = @id;";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@id", plataforma.Id);
        AdicionarParametro(comando, "@nome", plataforma.Nome);
        AdicionarParametro(comando, "@fabricante", plataforma.Fabricante);
        AdicionarParametro(comando, "@anoLancamento", plataforma.AnoLancamento);

        await ExecutarGravacaoAsync(comando);
    }

    public async Task ExcluirAsync(int id)
    {
        const string sql = @"
            DELETE FROM tb_plataforma
            WHERE co_seq_plataforma = @id;";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@id", id);

        await ExecutarGravacaoAsync(comando);
    }

    private static Plataforma MapearPlataforma(MySqlDataReader leitor)
    {
        return new Plataforma
        {
            Id = leitor.GetInt32("co_seq_plataforma"),
            Nome = leitor.GetString("no_plataforma"),
            Fabricante = leitor.GetString("no_fabricante"),
            AnoLancamento = leitor.GetInt32("nu_ano_lancamento")
        };
    }
}
