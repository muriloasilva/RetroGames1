namespace RetroGames.Repository;

public class RegistroDuplicadoException : Exception
{
    public RegistroDuplicadoException(string mensagem, Exception? innerException = null)
        : base(mensagem, innerException)
    {
    }
}
