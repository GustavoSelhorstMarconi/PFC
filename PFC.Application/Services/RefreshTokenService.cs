using Microsoft.Extensions.Configuration;
using PFC.Application.Interfaces;
using PFC.Domain.Entities;
using PFC.Domain.Interfaces;
using System.Globalization;
using System.Security.Cryptography;

namespace PFC.Application.Services;

public sealed class RefreshTokenService : IRefreshTokenService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IConfiguration _configuration;

    public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository, IConfiguration configuration)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _configuration = configuration;
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public async Task StoreRefreshTokenAsync(Guid userId, string token, CancellationToken cancellationToken)
    {
        var refreshDaysStr = _configuration["JwtSettings:RefreshTokenExpirationDays"] ?? "7";
        if (!int.TryParse(refreshDaysStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out var refreshDays))
            refreshDays = 7;

        TimeSpan validFor = TimeSpan.FromDays(refreshDays);

        var refreshToken = RefreshToken.Create(userId, token, validFor);
        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
    }

    public async Task<Guid?> ValidateRefreshTokenAsync(string token, CancellationToken cancellationToken)
    {
        var refreshToken = await _refreshTokenRepository.GetByTokenAsync(token, cancellationToken);

        if (refreshToken is null || !refreshToken.IsValid())
            return null;

        return refreshToken.UserId;
    }

    public async Task RevokeRefreshTokenAsync(string token, CancellationToken cancellationToken)
    {
        var refreshToken = await _refreshTokenRepository.GetByTokenAsync(token, cancellationToken);
        if (refreshToken is null)
            return;

        refreshToken.Revoke();
        await _refreshTokenRepository.UpdateAsync(refreshToken, cancellationToken);
    }

    public async Task RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken)
    {
        await _refreshTokenRepository.RevokeAllByUserIdAsync(userId, cancellationToken);
    }
}
