using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GeneroController : ControllerBase
{
    private readonly IGeneroService _generoService;

    public GeneroController(IGeneroService generoService)
    {
        _generoService = generoService;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var generos = await _generoService.ListarAsync();
        var dados = generos.Select(g => g.ParaResponse()).ToList();

        return Ok(new RetroGamesResponseApiModel<List<ListarGeneroResponseApiModel>>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = dados
        });
    }
}
