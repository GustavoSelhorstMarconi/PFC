namespace PFC.Domain.Exceptions;

public class UnauthorizedException : DomainException
{
    public UnauthorizedException(string message) : base(message)
    {
    }

    public UnauthorizedException()
        : base("Você não tem permissão para acessar este recurso.")
    {
    }
}
