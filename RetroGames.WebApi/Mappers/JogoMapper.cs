using RetroGames.Services;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Mappers;

public static class JogoMapper
{
    public static ListarJogoResponseApiModel ParaResponse(this JogoDetalheServiceModel detalhe)
    {
        return new ListarJogoResponseApiModel
        {
            Id = detalhe.Jogo.Id,
            Nome = detalhe.Jogo.Nome,
            AnoLancamento = detalhe.Jogo.AnoLancamento,
            Preco = detalhe.Jogo.Preco,
            Estoque = detalhe.Jogo.Estoque,
            Ativo = detalhe.Jogo.Ativo,
            NomePlataforma = detalhe.NomePlataforma,
            DescricaoCondicao = detalhe.DescricaoCondicao,
            Generos = detalhe.Generos.Select(g => g.Nome).ToList()
        };
    }

    public static ListarPorIdJogoResponseApiModel ParaDetalheResponse(this JogoDetalheServiceModel detalhe)
    {
        return new ListarPorIdJogoResponseApiModel
        {
            Id = detalhe.Jogo.Id,
            Nome = detalhe.Jogo.Nome,
            Descricao = detalhe.Jogo.Descricao,
            AnoLancamento = detalhe.Jogo.AnoLancamento,
            Preco = detalhe.Jogo.Preco,
            Estoque = detalhe.Jogo.Estoque,
            Ativo = detalhe.Jogo.Ativo,
            IdPlataforma = detalhe.Jogo.IdPlataforma,
            NomePlataforma = detalhe.NomePlataforma,
            IdCondicaoJogo = detalhe.Jogo.IdCondicaoJogo,
            DescricaoCondicao = detalhe.DescricaoCondicao,
            Generos = detalhe.Generos.Select(g => g.ParaResponse()).ToList()
        };
    }

    public static PaginaResponseApiModel<ListarJogoResponseApiModel> ParaPaginaResponse(
        this (List<JogoDetalheServiceModel> Jogos, int Total) resultado,
        ListarJogoRequestApiModel filtro)
    {
        var pagina = filtro.Pagina < 1 ? 1 : filtro.Pagina;
        var tamanhoPagina = filtro.TamanhoPagina < 1 ? 12 : filtro.TamanhoPagina;
        var totalPaginas = resultado.Total == 0
            ? 0
            : (int)Math.Ceiling((double)resultado.Total / tamanhoPagina);

        return new PaginaResponseApiModel<ListarJogoResponseApiModel>
        {
            Itens = resultado.Jogos.Select(j => j.ParaResponse()).ToList(),
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina,
            TotalItens = resultado.Total,
            TotalPaginas = totalPaginas
        };
    }
}
