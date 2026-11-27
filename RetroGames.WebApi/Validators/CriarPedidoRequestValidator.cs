using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators;

public class CriarPedidoRequestValidator : AbstractValidator<CriarPedidoRequestApiModel>
{
    public CriarPedidoRequestValidator()
    {
        RuleFor(x => x.Itens)
            .NotEmpty().WithMessage("Adicione pelo menos um item ao pedido.");

        RuleForEach(x => x.Itens)
            .Must(item => item.Key > 0).WithMessage("Jogo inválido.")
            .Must(item => item.Value > 0).WithMessage("A quantidade deve ser maior que zero.");
    }
}
