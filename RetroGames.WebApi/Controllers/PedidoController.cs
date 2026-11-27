using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidoController : ControllerBase
{
    private readonly IPedidoService _pedidoService;
    private readonly IUsuarioAtualService _usuarioAtual;

    public PedidoController(IPedidoService pedidoService, IUsuarioAtualService usuarioAtual)
    {
        _pedidoService = pedidoService;
        _usuarioAtual = usuarioAtual;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Criar(CriarPedidoRequestApiModel request)
    {
        if (_usuarioAtual.Id is null)
        {
            return StatusCode(StatusCodes.Status401Unauthorized,
                new RetroGamesResponseApiModel
                {
                    ResponseCode = ResponseCodes.NAO_AUTENTICADO,
                    ErrorMessage = "Usuário não autenticado."
                });
        }

        var detalhe = await _pedidoService.CriarAsync(_usuarioAtual.Id.Value, request.Itens);
        var dados = ParaResponse(detalhe);

        return Ok(new RetroGamesResponseApiModel<PedidoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = dados
        });
    }

    [Authorize]
    [HttpGet("meus")]
    public async Task<IActionResult> Meus()
    {
        if (_usuarioAtual.Id is null)
        {
            return StatusCode(StatusCodes.Status401Unauthorized,
                new RetroGamesResponseApiModel
                {
                    ResponseCode = ResponseCodes.NAO_AUTENTICADO,
                    ErrorMessage = "Usuário não autenticado."
                });
        }

        var detalhes = await _pedidoService.ListarPorUsuarioAsync(_usuarioAtual.Id.Value);
        var dados = detalhes.Select(ParaResponse).ToList();

        return Ok(new RetroGamesResponseApiModel<List<PedidoResponseApiModel>>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = dados
        });
    }

    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        if (_usuarioAtual.Id is null)
        {
            return StatusCode(StatusCodes.Status401Unauthorized,
                new RetroGamesResponseApiModel
                {
                    ResponseCode = ResponseCodes.NAO_AUTENTICADO,
                    ErrorMessage = "Usuário não autenticado."
                });
        }

        var detalhe = await _pedidoService.BuscarPorIdAsync(_usuarioAtual.Id.Value, id);

        return Ok(new RetroGamesResponseApiModel<PedidoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = ParaResponse(detalhe)
        });
    }

    private static PedidoResponseApiModel ParaResponse(PedidoDetalheServiceModel detalhe)
    {
        return new PedidoResponseApiModel
        {
            Id = detalhe.Pedido.Id,
            DataPedido = detalhe.Pedido.DataPedido,
            Total = detalhe.Pedido.Total,
            Situacao = detalhe.Situacao,
            Itens = detalhe.Itens.Select(item => new ItemPedidoResponseApiModel
            {
                IdJogo = item.IdJogo,
                NomeJogo = item.NomeJogo,
                PrecoUnitario = item.PrecoUnitario,
                Quantidade = item.Quantidade,
                Subtotal = item.Subtotal
            }).ToList()
        };
    }
}
