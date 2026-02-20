using PFC.Domain.Entities;

namespace PFC.Domain.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken);
    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
    Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
    Task RevokeAllByUserIdAsync(Guid userId, CancellationToken cancellationToken);
}
