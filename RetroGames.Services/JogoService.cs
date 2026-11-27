using System.Transactions;
using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public class JogoService : IJogoService
{
    private readonly IRepositorySet _repositorio;

    public JogoService(IRepositorySet repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<(List<JogoDetalheServiceModel> Jogos, int Total)> ListarAsync(
        string? busca = null,
        int? idPlataforma = null,
        int? idGenero = null,
        bool incluirInativos = false,
        int pagina = 1,
        int tamanhoPagina = 10)
    {
        List<int>? idsJogosFiltro = null;

        if (idGenero.HasValue)
        {
            var todosVinculos = await _repositorio.JogoGenero.ListarTodosAsync();
            idsJogosFiltro = todosVinculos
                .Where(v => v.IdGenero == idGenero.Value)
                .Select(v => v.IdJogo)
                .Distinct()
                .ToList();
        }

        var (jogos, total) = await _repositorio.Jogo.ListarAsync(
            busca: busca,
            idPlataforma: idPlataforma,
            idsJogos: idsJogosFiltro,
            incluirInativos: incluirInativos,
            pagina: pagina,
            tamanhoPagina: tamanhoPagina);

        var detalhes = await MontarDetalhesAsync(jogos);
        return (detalhes, total);
    }

    public async Task<JogoDetalheServiceModel> BuscarPorIdAsync(int id, bool incluirInativos = false)
    {
        var jogo = await _repositorio.Jogo.BuscarPorIdAsync(id);
        if (jogo is null || (!incluirInativos && !jogo.Ativo))
        {
            throw new BusinessRuleException(BusinessErrors.JogoNaoEncontrado);
        }

        var detalhes = await MontarDetalhesAsync([jogo]);
        return detalhes.First();
    }

    public async Task<JogoDetalheServiceModel> InserirAsync(Jogo jogo, List<int> idsGeneros)
    {
        var generosDistintos = idsGeneros.Distinct().ToList();
        await ValidarReferenciasAsync(jogo.IdPlataforma, jogo.IdCondicaoJogo, generosDistintos);

        jogo.Nome = jogo.Nome.Trim();
        jogo.Ativo = true;
        jogo.DataCadastro = DateTime.Now;

        using (var transacao = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            jogo.Id = await _repositorio.Jogo.InserirAsync(jogo);
            await _repositorio.JogoGenero.InserirAsync(jogo.Id, generosDistintos);
            transacao.Complete();
        }

        return await BuscarPorIdAsync(jogo.Id);
    }

    public async Task AlterarAsync(Jogo jogo, List<int> idsGeneros)
    {
        var existente = await _repositorio.Jogo.BuscarPorIdAsync(jogo.Id);
        if (existente is null)
        {
            throw new BusinessRuleException(BusinessErrors.JogoNaoEncontrado);
        }

        var generosDistintos = idsGeneros.Distinct().ToList();
        await ValidarReferenciasAsync(jogo.IdPlataforma, jogo.IdCondicaoJogo, generosDistintos);

        jogo.Nome = jogo.Nome.Trim();
        jogo.Ativo = existente.Ativo;

        using (var transacao = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            await _repositorio.Jogo.AlterarAsync(jogo);
            await _repositorio.JogoGenero.RemoverPorJogoAsync(jogo.Id);
            await _repositorio.JogoGenero.InserirAsync(jogo.Id, generosDistintos);
            transacao.Complete();
        }
    }

    public async Task DesativarAsync(int id)
    {
        var existente = await _repositorio.Jogo.BuscarPorIdAsync(id);
        if (existente is null)
        {
            throw new BusinessRuleException(BusinessErrors.JogoNaoEncontrado);
        }

        await _repositorio.Jogo.DesativarAsync(id);
    }

    private async Task ValidarReferenciasAsync(int idPlataforma, int idCondicaoJogo, List<int> idsGeneros)
    {
        var plataforma = await _repositorio.Plataforma.BuscarPorIdAsync(idPlataforma);
        if (plataforma is null)
        {
            throw new BusinessRuleException(BusinessErrors.PlataformaNaoEncontrada);
        }

        var condicoes = await _repositorio.CondicaoJogo.ListarAsync();
        if (!condicoes.Any(c => c.Id == idCondicaoJogo))
        {
            throw new BusinessRuleException(BusinessErrors.CondicaoNaoEncontrada);
        }

        if (idsGeneros.Count > 0)
        {
            var generosExistentes = await _repositorio.Genero.ListarAsync();
            var idsExistentes = generosExistentes.Select(g => g.Id).ToHashSet();

            if (idsGeneros.Any(id => !idsExistentes.Contains(id)))
            {
                throw new BusinessRuleException(BusinessErrors.GeneroNaoEncontrado);
            }
        }
    }

    private async Task<List<JogoDetalheServiceModel>> MontarDetalhesAsync(List<Jogo> jogos)
    {
        if (jogos.Count == 0)
        {
            return new List<JogoDetalheServiceModel>();
        }

        var plataformas = await _repositorio.Plataforma.ListarAsync();
        var condicoes = await _repositorio.CondicaoJogo.ListarAsync();
        var generos = await _repositorio.Genero.ListarAsync();

        var idsJogos = jogos.Select(j => j.Id).ToList();
        var vinculos = await _repositorio.JogoGenero.ListarPorJogosAsync(idsJogos);

        var mapaPlataformas = plataformas.ToDictionary(p => p.Id, p => p.Nome);
        var mapaCondicoes = condicoes.ToDictionary(c => c.Id, c => c.Descricao);
        var mapaGeneros = generos.ToDictionary(g => g.Id);

        return jogos.Select(jogo => new JogoDetalheServiceModel
        {
            Jogo = jogo,
            NomePlataforma = mapaPlataformas.GetValueOrDefault(jogo.IdPlataforma, string.Empty),
            DescricaoCondicao = mapaCondicoes.GetValueOrDefault(jogo.IdCondicaoJogo, string.Empty),
            Generos = vinculos
                .Where(v => v.IdJogo == jogo.Id)
                .Select(v => mapaGeneros.GetValueOrDefault(v.IdGenero))
                .Where(g => g is not null)
                .Cast<Genero>()
                .OrderBy(g => g.Nome)
                .ToList()
        }).ToList();
    }
}
