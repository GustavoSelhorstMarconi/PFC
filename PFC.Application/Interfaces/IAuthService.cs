using PFC.Application.Common;
using PFC.Dto.Auth;

namespace PFC.Application.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
    Task<Result<AuthResponse>> RefreshAsync(RefreshRequest request, CancellationToken cancellationToken);
    Task<Result> RevokeAsync(RefreshRequest request, CancellationToken cancellationToken);
}
