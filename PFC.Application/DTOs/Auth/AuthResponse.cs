namespace PFC.Application.DTOs.Auth;

public sealed class AuthResponse
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}
