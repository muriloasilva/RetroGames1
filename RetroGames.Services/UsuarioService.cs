using Microsoft.AspNetCore.Identity;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IRepositorySet _repositorio;
    private readonly IPasswordHasher<Usuario> _passwordHasher;

    public UsuarioService(IRepositorySet repositorio, IPasswordHasher<Usuario> passwordHasher)
    {
        _repositorio = repositorio;
        _passwordHasher = passwordHasher;
    }

    public async Task<Usuario> CadastrarAsync(Usuario usuario, string senha)
    {
        usuario.Nome = usuario.Nome.Trim();
        usuario.Email = usuario.Email.Trim().ToLowerInvariant();

        if (await _repositorio.Usuario.ExisteComEmailAsync(usuario.Email))
        {
            throw new BusinessRuleException(BusinessErrors.EmailJaCadastrado);
        }

        usuario.Ativo = true;
        usuario.DataCadastro = DateTime.UtcNow;
        usuario.IdPerfil = PerfilIds.Cliente;
        usuario.SenhaHash = _passwordHasher.HashPassword(usuario, senha);

        try
        {
            usuario.Id = await _repositorio.Usuario.InserirAsync(usuario);
            return usuario;
        }
        catch (RegistroDuplicadoException)
        {
            throw new BusinessRuleException(BusinessErrors.EmailJaCadastrado);
        }
    }
}
