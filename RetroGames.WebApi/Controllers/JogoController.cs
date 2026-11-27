using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JogoController : ControllerBase
{
    private readonly IJogoService _jogoService;
    private readonly IUsuarioAtualService _usuarioAtual;

    public JogoController(IJogoService jogoService, IUsuarioAtualService usuarioAtual)
    {
        _jogoService = jogoService;
        _usuarioAtual = usuarioAtual;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] ListarJogoRequestApiModel request)
    {
        var resultado = await _jogoService.ListarAsync(
            busca: request.Busca,
            idPlataforma: request.IdPlataforma,
            idGenero: request.IdGenero,
            incluirInativos: _usuarioAtual.EAdministrador,
            pagina: request.Pagina,
            tamanhoPagina: request.TamanhoPagina);

        return Ok(new RetroGamesResponseApiModel<PaginaResponseApiModel<ListarJogoResponseApiModel>>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = resultado.ParaPaginaResponse(request)
        });
    }

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> ListarPorId(int id)
    {
        var detalhe = await _jogoService.BuscarPorIdAsync(id, _usuarioAtual.EAdministrador);

        return Ok(new RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = detalhe.ParaDetalheResponse()
        });
    }

    [Authorize(Policy = "SomenteAdministrador")]
    [HttpPost]
    public async Task<IActionResult> Inserir(InserirJogoRequestApiModel request)
    {
        var criado = await _jogoService.InserirAsync(request.ParaModel(), request.IdsGeneros);

        return CreatedAtAction(nameof(ListarPorId), new { id = criado.Jogo.Id },
            new RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>
            {
                ResponseCode = ResponseCodes.SUCESSO,
                Data = criado.ParaDetalheResponse()
            });
    }

    [Authorize(Policy = "SomenteAdministrador")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Alterar(int id, AlterarJogoRequestApiModel request)
    {
        await _jogoService.AlterarAsync(request.ParaModel(id), request.IdsGeneros);

        var alterado = await _jogoService.BuscarPorIdAsync(id, incluirInativos: true);
        return Ok(new RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = alterado.ParaDetalheResponse()
        });
    }

    [Authorize(Policy = "SomenteAdministrador")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Desativar(int id)
    {
        await _jogoService.DesativarAsync(id);

        return Ok(new RetroGamesResponseApiModel
        {
            ResponseCode = ResponseCodes.SUCESSO
        });
    }
}
