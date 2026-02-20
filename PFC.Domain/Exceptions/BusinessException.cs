namespace PFC.Domain.Exceptions;

public class BusinessException : DomainException
{
    public BusinessException(string message) : base(message)
    {
    }

    public BusinessException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
