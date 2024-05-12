using CultureStay.Domain.Entities;

namespace CultureStay.Application.Services.Interface;

public interface ITokenService
{
    string GenerateToken(User user);
    public (string, DateTime) GenerateRefreshToken();
    Task<RefreshToken> ValidateRefreshTokenAsync(string refreshToken);
    Task RevokeRefreshTokenAsync(string refreshToken);
    Task SaveRefreshTokenAsync(string refreshToken, int userId, DateTime? expires = null);
}