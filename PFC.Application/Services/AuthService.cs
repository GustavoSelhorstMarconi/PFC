using PFC.Application.Common;
using PFC.Application.Interfaces;
using PFC.Domain.Entities;
using PFC.Domain.Exceptions;
using PFC.Domain.Interfaces;
using PFC.Domain.ValueObjects;
using PFC.Dto.Auth;

namespace PFC.Application.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IRefreshTokenService _refreshTokenService;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IRefreshTokenService refreshTokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null)
            throw new BadRequestException("Invalid request");

        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.Name))
            throw new BadRequestException("Name, email and password are required");

        var existing = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existing is not null)
            throw new ConflictException("Email já cadastrado");

        var passwordHash = _passwordHasher.HashPassword(request.Password);
        var email = new Email(request.Email);
        var user = new User(request.Name, email, passwordHash);

        await _userRepository.AddAsync(user, cancellationToken);

        var accessToken = _jwtTokenGenerator.GenerateAccessToken(user.Id, user.Email, user.Name);

        var refreshToken = _refreshTokenService.GenerateRefreshToken();

        await _refreshTokenService.StoreRefreshTokenAsync(user.Id, refreshToken, cancellationToken);

        var response = new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

        return Result.Success(response);
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null)
            throw new BadRequestException("Invalid request");

        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            throw new BadRequestException("Email and password are required");

        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
            throw new UnauthorizedException("Credenciais inválidas");

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Credenciais inválidas");

        await _refreshTokenService.RevokeAllUserTokensAsync(user.Id, cancellationToken);

        var accessToken = _jwtTokenGenerator.GenerateAccessToken(user.Id, user.Email, user.Name);

        var refreshToken = _refreshTokenService.GenerateRefreshToken();

        await _refreshTokenService.StoreRefreshTokenAsync(user.Id, refreshToken, cancellationToken);

        var response = new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

        return Result.Success(response);
    }

    public async Task<Result<AuthResponse>> RefreshAsync(RefreshRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.RefreshToken))
            throw new BadRequestException("Refresh token is required");

        var userId = await _refreshTokenService.ValidateRefreshTokenAsync(request.RefreshToken, cancellationToken);
        if (userId is null)
            throw new UnauthorizedException("Invalid refresh token");

        var user = await _userRepository.GetByIdAsync(userId.Value, cancellationToken);
        if (user is null)
            throw new NotFoundException("User not found");

        await _refreshTokenService.RevokeRefreshTokenAsync(request.RefreshToken, cancellationToken);

        var accessToken = _jwtTokenGenerator.GenerateAccessToken(user.Id, user.Email, user.Name);

        var newRefreshToken = _refreshTokenService.GenerateRefreshToken();

        await _refreshTokenService.StoreRefreshTokenAsync(user.Id, newRefreshToken, cancellationToken);

        var response = new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken
        };

        return Result.Success(response);
    }

    public async Task<Result> RevokeAsync(RefreshRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.RefreshToken))
            throw new BadRequestException("Refresh token is required");

        await _refreshTokenService.RevokeRefreshTokenAsync(request.RefreshToken, cancellationToken);

        return Result.Success();
    }
}
