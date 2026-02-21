namespace PFC.Domain.Interfaces;

public interface ICurrentUserService
{
    Guid GetUserId();

    string? GetUserEmail();

    string? GetUserName();

    bool IsAuthenticated();
}