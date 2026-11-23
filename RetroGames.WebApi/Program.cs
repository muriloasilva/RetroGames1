using DotNetEnv;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Configuration;
using RetroGames.WebApi.Filters;
using RetroGames.WebApi.Middlewares;

Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "A string de conexão 'ConnectionStrings:Default' não foi encontrada. Verifique o arquivo .env na raiz do projeto.");
}

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddRepositorios(connectionString);
builder.Services.AddServicosDeNegocio();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddControllers(opcoes =>
{
    opcoes.Filters.Add<FluentValidationFilter>();
})
.ConfigureApiBehaviorOptions(opcoes =>
{
    opcoes.InvalidModelStateResponseFactory = context =>
    {
        var errosPorCampo = context.ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value!.Errors.Select(e =>
                    string.IsNullOrWhiteSpace(e.ErrorMessage) ? "Valor inválido." : e.ErrorMessage).ToArray());

        var envelope = new RetroGamesResponseApiModel<Dictionary<string, string[]>>
        {
            ResponseCode = ResponseCodes.ERRO_VALIDACAO,
            ErrorMessage = "Requisição malformada ou dados inválidos.",
            Data = errosPorCampo
        };

        return new BadRequestObjectResult(envelope);
    };
});

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "RetroGames API v1");
    });
}

app.MapControllers();

app.Run();
