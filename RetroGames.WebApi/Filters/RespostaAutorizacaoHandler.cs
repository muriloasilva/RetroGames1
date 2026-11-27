using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Filters;

public class RespostaAutorizacaoHandler : IAuthorizationMiddlewareResultHandler
{
    public async Task HandleAsync(
        RequestDelegate proximo,
        HttpContext contexto,
        AuthorizationPolicy politica,
        PolicyAuthorizationResult resultado)
    {
        if (resultado.Challenged)
        {
            contexto.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await contexto.Response.WriteAsJsonAsync(new RetroGamesResponseApiModel
            {
                ResponseCode = ResponseCodes.NAO_AUTENTICADO,
                ErrorMessage = "Usuário não autenticado."
            });
            return;
        }

        if (resultado.Forbidden)
        {
            contexto.Response.StatusCode = StatusCodes.Status403Forbidden;
            await contexto.Response.WriteAsJsonAsync(new RetroGamesResponseApiModel
            {
                ResponseCode = ResponseCodes.ACESSO_NEGADO,
                ErrorMessage = "Acesso negado."
            });
            return;
        }

        await proximo(contexto);
    }
}
