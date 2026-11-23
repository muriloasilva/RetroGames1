using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Filters;

public class FluentValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var argumento in context.ActionArguments.Values)
        {
            if (argumento is null)
            {
                continue;
            }

            var validatorType = typeof(IValidator<>).MakeGenericType(argumento.GetType());
            if (context.HttpContext.RequestServices.GetService(validatorType) is IValidator validator)
            {
                var validationContext = new ValidationContext<object>(argumento);
                var resultado = await validator.ValidateAsync(validationContext);

                if (!resultado.IsValid)
                {
                    var errosPorCampo = resultado.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToArray());

                    var envelope = new RetroGamesResponseApiModel<Dictionary<string, string[]>>
                    {
                        ResponseCode = ResponseCodes.ERRO_VALIDACAO,
                        ErrorMessage = "Dados de entrada inválidos.",
                        Data = errosPorCampo
                    };

                    context.Result = new BadRequestObjectResult(envelope);
                    return;
                }
            }
        }

        await next();
    }
}
