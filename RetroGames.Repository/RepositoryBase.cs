using MySqlConnector;

namespace RetroGames.Repository;

public abstract class RepositoryBase
{
    private readonly string _connectionString;

    protected RepositoryBase(RepositoryConfig config)
    {
        _connectionString = config.ConnectionString;
    }

    protected async Task<MySqlConnection> AbrirConexaoAsync()
    {
        var conexao = new MySqlConnection(_connectionString);
        await conexao.OpenAsync();
        return conexao;
    }

    protected static MySqlCommand CriarComando(MySqlConnection conexao, string sql)
    {
        var comando = conexao.CreateCommand();
        comando.CommandText = sql;
        return comando;
    }

    protected static void AdicionarParametro(MySqlCommand comando, string nome, object? valor)
    {
        comando.Parameters.AddWithValue(nome, valor ?? DBNull.Value);
    }

    protected static async Task<long> ExecutarGravacaoAsync(MySqlCommand comando)
    {
        try
        {
            await comando.ExecuteNonQueryAsync();
            return comando.LastInsertedId;
        }
        catch (MySqlException ex) when (ex.Number == 1062)
        {
            throw new RegistroDuplicadoException("Registro duplicado no banco de dados.", ex);
        }
    }
}
