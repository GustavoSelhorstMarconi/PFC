namespace PFC.Application.Interfaces;

public interface IRefreshTokenService
{
    string GenerateRefreshToken();
    Task StoreRefreshTokenAsync(Guid userId, string token, CancellationToken cancellationToken);
    Task<Guid?> ValidateRefreshTokenAsync(string token, CancellationToken cancellationToken);
    Task RevokeRefreshTokenAsync(string token, CancellationToken cancellationToken);
    Task RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken);
}
