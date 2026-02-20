namespace PFC.Domain.Exceptions;

public class ForbiddenException : DomainException
{
    public ForbiddenException(string message) : base(message)
    {
    }

    public ForbiddenException()
        : base("Acesso negado a este recurso.")
    {
    }
}
