using MySqlConnector;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class JogoRepository : RepositoryBase, IJogoRepository
{
    public JogoRepository(RepositoryConfig config) : base(config)
    {
    }

    public async Task<(List<Jogo> Jogos, int Total)> ListarAsync(
        string? busca = null,
        int? idPlataforma = null,
        List<int>? idsJogos = null,
        bool incluirInativos = false,
        int pagina = 1,
        int tamanhoPagina = 10)
    {
        if (idsJogos is not null && idsJogos.Count == 0)
        {
            return (new List<Jogo>(), 0);
        }

        var condicoes = new List<string>();
        if (!incluirInativos)
        {
            condicoes.Add("st_ativo = TRUE");
        }
        if (!string.IsNullOrWhiteSpace(busca))
        {
            condicoes.Add("no_jogo LIKE @busca");
        }
        if (idPlataforma.HasValue)
        {
            condicoes.Add("co_plataforma = @idPlataforma");
        }
        if (idsJogos is not null && idsJogos.Count > 0)
        {
            var nomesParametros = idsJogos.Select((_, i) => $"@idJogo{i}");
            condicoes.Add($"co_seq_jogo IN ({string.Join(", ", nomesParametros)})");
        }

        var where = condicoes.Count > 0
            ? "WHERE " + string.Join(" AND ", condicoes)
            : string.Empty;

        var sqlTotal = $"SELECT COUNT(1) FROM tb_jogo {where};";

        var sqlDados = $@"
            SELECT co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco,
                   qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo
            FROM tb_jogo
            {where}
            ORDER BY no_jogo
            LIMIT @limite OFFSET @deslocamento;";

        await using var conexao = await AbrirConexaoAsync();

        await using var comandoTotal = CriarComando(conexao, sqlTotal);
        PreencherFiltros(comandoTotal, busca, idPlataforma, idsJogos);
        var total = Convert.ToInt32(await comandoTotal.ExecuteScalarAsync());

        await using var comandoDados = CriarComando(conexao, sqlDados);
        PreencherFiltros(comandoDados, busca, idPlataforma, idsJogos);
        AdicionarParametro(comandoDados, "@limite", tamanhoPagina);
        AdicionarParametro(comandoDados, "@deslocamento", (pagina - 1) * tamanhoPagina);

        await using var leitor = await comandoDados.ExecuteReaderAsync();
        var jogos = new List<Jogo>();
        while (await leitor.ReadAsync())
        {
            jogos.Add(MapearJogo(leitor));
        }

        return (jogos, total);
    }

    public async Task<Jogo?> BuscarPorIdAsync(int id)
    {
        const string sql = @"
            SELECT co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco,
                   qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo
            FROM tb_jogo
            WHERE co_seq_jogo = @id;";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@id", id);

        await using var leitor = await comando.ExecuteReaderAsync();
        if (await leitor.ReadAsync())
        {
            return MapearJogo(leitor);
        }

        return null;
    }

    public async Task<int> InserirAsync(Jogo jogo)
    {
        const string sql = @"
            INSERT INTO tb_jogo (
                no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque,
                st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo
            )
            VALUES (
                @nome, @descricao, @anoLancamento, @preco, @estoque,
                @ativo, @dataCadastro, @idPlataforma, @idCondicaoJogo
            );";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@nome", jogo.Nome);
        AdicionarParametro(comando, "@descricao", jogo.Descricao);
        AdicionarParametro(comando, "@anoLancamento", jogo.AnoLancamento);
        AdicionarParametro(comando, "@preco", jogo.Preco);
        AdicionarParametro(comando, "@estoque", jogo.Estoque);
        AdicionarParametro(comando, "@ativo", jogo.Ativo);
        AdicionarParametro(comando, "@dataCadastro", jogo.DataCadastro);
        AdicionarParametro(comando, "@idPlataforma", jogo.IdPlataforma);
        AdicionarParametro(comando, "@idCondicaoJogo", jogo.IdCondicaoJogo);

        var idGerado = await ExecutarGravacaoAsync(comando);
        return (int)idGerado;
    }

    public async Task AlterarAsync(Jogo jogo)
    {
        const string sql = @"
            UPDATE tb_jogo
            SET no_jogo = @nome,
                ds_jogo = @descricao,
                nu_ano_lancamento = @anoLancamento,
                vl_preco = @preco,
                qt_estoque = @estoque,
                st_ativo = @ativo,
                co_plataforma = @idPlataforma,
                co_condicao_jogo = @idCondicaoJogo
            WHERE co_seq_jogo = @id;";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@id", jogo.Id);
        AdicionarParametro(comando, "@nome", jogo.Nome);
        AdicionarParametro(comando, "@descricao", jogo.Descricao);
        AdicionarParametro(comando, "@anoLancamento", jogo.AnoLancamento);
        AdicionarParametro(comando, "@preco", jogo.Preco);
        AdicionarParametro(comando, "@estoque", jogo.Estoque);
        AdicionarParametro(comando, "@ativo", jogo.Ativo);
        AdicionarParametro(comando, "@idPlataforma", jogo.IdPlataforma);
        AdicionarParametro(comando, "@idCondicaoJogo", jogo.IdCondicaoJogo);

        await ExecutarGravacaoAsync(comando);
    }

    public async Task DesativarAsync(int id)
    {
        const string sql = @"
            UPDATE tb_jogo
            SET st_ativo = FALSE
            WHERE co_seq_jogo = @id;";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@id", id);

        await ExecutarGravacaoAsync(comando);
    }

    private static void PreencherFiltros(
        MySqlCommand comando,
        string? busca,
        int? idPlataforma,
        List<int>? idsJogos)
    {
        if (!string.IsNullOrWhiteSpace(busca))
        {
            AdicionarParametro(comando, "@busca", $"%{busca}%");
        }
        if (idPlataforma.HasValue)
        {
            AdicionarParametro(comando, "@idPlataforma", idPlataforma.Value);
        }
        if (idsJogos is not null && idsJogos.Count > 0)
        {
            for (var i = 0; i < idsJogos.Count; i++)
            {
                AdicionarParametro(comando, $"@idJogo{i}", idsJogos[i]);
            }
        }
    }

    private static Jogo MapearJogo(MySqlDataReader leitor)
    {
        var indiceDescricao = leitor.GetOrdinal("ds_jogo");

        return new Jogo
        {
            Id = leitor.GetInt32("co_seq_jogo"),
            Nome = leitor.GetString("no_jogo"),
            Descricao = leitor.IsDBNull(indiceDescricao) ? null : leitor.GetString(indiceDescricao),
            AnoLancamento = leitor.GetInt32("nu_ano_lancamento"),
            Preco = leitor.GetDecimal("vl_preco"),
            Estoque = leitor.GetInt32("qt_estoque"),
            Ativo = leitor.GetBoolean("st_ativo"),
            DataCadastro = leitor.GetDateTime("dt_cadastro"),
            IdPlataforma = leitor.GetInt32("co_plataforma"),
            IdCondicaoJogo = leitor.GetInt32("co_condicao_jogo")
        };
    }
}
