using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Extensions;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AutenticacaoController : ControllerBase
{
    private readonly IAutenticacaoService _autenticacaoService;
    private readonly IUsuarioService _usuarioService;

    public AutenticacaoController(IAutenticacaoService autenticacaoService, IUsuarioService usuarioService)
    {
        _autenticacaoService = autenticacaoService;
        _usuarioService = usuarioService;
    }

    [AllowAnonymous]
    [HttpPost("cadastrar")]
    public async Task<IActionResult> Cadastrar(CadastrarUsuarioRequestApiModel request)
    {
        var usuario = await _usuarioService.CadastrarAsync(new Usuario
        {
            Nome = request.Nome,
            Email = request.Email
        }, request.Senha);

        var dados = new UsuarioResponseApiModel
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Perfil = "Cliente"
        };

        return Ok(new RetroGamesResponseApiModel<UsuarioResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = dados
        });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestApiModel request)
    {
        var usuario = await _autenticacaoService.AutenticarAsync(request.Email, request.Senha);
        if (usuario is null)
        {
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            return new UnauthorizedObjectResult(new RetroGamesResponseApiModel
            {
                ResponseCode = ResponseCodes.CREDENCIAIS_INVALIDAS,
                ErrorMessage = "E-mail ou senha inválidos."
            });
        }

        var perfil = usuario.IdPerfil == PerfilIds.Administrador ? "Administrador" : "Cliente";
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Email, usuario.Email),
            new(ClaimTypes.Role, perfil)
        };

        var identidade = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identidade);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = false,
                AllowRefresh = true
            });

        var dados = new LoginResponseApiModel
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Perfil = perfil
        };

        return Ok(new RetroGamesResponseApiModel<LoginResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = dados
        });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return Ok(new RetroGamesResponseApiModel
        {
            ResponseCode = ResponseCodes.SUCESSO
        });
    }

    [Authorize]
    [HttpGet("eu")]
    public async Task<IActionResult> Eu()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            return new UnauthorizedObjectResult(new RetroGamesResponseApiModel
            {
                ResponseCode = ResponseCodes.NAO_AUTENTICADO,
                ErrorMessage = "Usuário não autenticado."
            });
        }

        var id = User.ObterUsuarioId();
        if (id is null)
        {
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            return new UnauthorizedObjectResult(new RetroGamesResponseApiModel
            {
                ResponseCode = ResponseCodes.NAO_AUTENTICADO,
                ErrorMessage = "Usuário não autenticado."
            });
        }

        var dados = new UsuarioResponseApiModel
        {
            Id = id.Value,
            Nome = User.ObterUsuarioNome(),
            Email = User.ObterUsuarioEmail(),
            Perfil = User.ObterUsuarioPerfil()
        };

        return Ok(new RetroGamesResponseApiModel<UsuarioResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = dados
        });
    }
}
