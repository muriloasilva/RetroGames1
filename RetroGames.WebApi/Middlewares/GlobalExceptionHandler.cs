using Microsoft.AspNetCore.Diagnostics;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Middlewares;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        int statusCode;
        RetroGamesResponseApiModel resposta;

        if (exception is BusinessRuleException regraException)
        {
            statusCode = (int)regraException.Erro.Status;
            resposta = new RetroGamesResponseApiModel
            {
                ResponseCode = regraException.Erro.Codigo,
                ErrorMessage = regraException.Erro.Mensagem
            };
        }
        else
        {
            _logger.LogError(exception, "Erro inesperado ao processar a requisição.");

            statusCode = StatusCodes.Status500InternalServerError;
            resposta = new RetroGamesResponseApiModel
            {
                ResponseCode = ResponseCodes.ERRO,
                ErrorMessage = "Ocorreu um erro interno inesperado. Tente novamente mais tarde."
            };
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(resposta, cancellationToken);

        return true;
    }
}
