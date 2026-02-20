namespace PFC.Domain.Exceptions;

public class ConflictException : DomainException
{
    public ConflictException(string message) : base(message)
    {
    }

    public ConflictException(string name, object key)
        : base($"{name} com id '{key}' já existe.")
    {
    }
}
