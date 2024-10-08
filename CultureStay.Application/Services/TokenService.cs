using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using CultureStay.Application.Common.Configurations;
using CultureStay.Application.Common.Interfaces;
using CultureStay.Application.Services.Interface;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using CultureStay.Domain.Entities;
using CultureStay.Domain.Exceptions;
using CultureStay.Domain.Repositories.Base;
using CultureStay.Domain.Resources;

namespace CultureStay.Application.Services;

public class TokenService(
	IOptionsMonitor<TokenSettings> tokenSettings,
	IRepositoryBase<RefreshToken> refreshTokenRepository,
	IUnitOfWork unitOfWork,
	IMapper mapper,
	ICurrentUser currentUser) : BaseService(unitOfWork, mapper, currentUser), ITokenService
{
	public string GenerateToken(User user)
	{
		var tokenHandler = new JwtSecurityTokenHandler();
		var key = tokenSettings.CurrentValue.Key;
		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(new[]
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
				new Claim(ClaimTypes.Name, user.FullName ?? string.Empty),
				new Claim(ClaimTypes.Uri, user.AvatarUrl ?? string.Empty),		
			}),
			Expires = DateTime.UtcNow.AddMinutes(tokenSettings.CurrentValue.ExpiryInMinutes),
			SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
				SecurityAlgorithms.HmacSha512Signature)
		};
		var token = tokenHandler.CreateToken(tokenDescriptor);
		return tokenHandler.WriteToken(token);
	}

	public (string, DateTime) GenerateRefreshToken()
		=> (Guid.NewGuid().ToString().Replace("-", ""),
			   DateTime.UtcNow.AddHours(tokenSettings.CurrentValue.RefreshTokenExpiryInHours));

	public async Task<RefreshToken> ValidateRefreshTokenAsync(string refreshToken)
	{
		var refreshTokenEntity = await refreshTokenRepository.GetAsync(e => e.Token == refreshToken);

		if (refreshTokenEntity == null)
			throw new NotFoundException(nameof(RefreshToken), refreshToken);

		if (refreshTokenEntity.IsExpired)
			throw new AppException(ErrorMessages.RefreshTokenExpired);

		return refreshTokenEntity;
	}

	public async Task RevokeRefreshTokenAsync(string refreshToken)
	{
		var refreshTokenEntity = await refreshTokenRepository.GetAsync(e => e.Token == refreshToken)
		                         ?? throw new NotFoundException(nameof(RefreshToken), refreshToken);

		refreshTokenRepository.Delete(refreshTokenEntity);
		await UnitOfWork.SaveChangesAsync();
	}
	
	public async Task SaveRefreshTokenAsync(string refreshToken, int userId, DateTime? expires = null)
	{
		expires ??= DateTime.UtcNow.AddHours(tokenSettings.CurrentValue.RefreshTokenExpiryInHours);
		var refreshTokenEntity = new RefreshToken
		{
			Token = refreshToken,
			UserId = userId,
			Expires = expires.Value
		};	
		
		refreshTokenRepository.Add(refreshTokenEntity);
		await UnitOfWork.SaveChangesAsync();
	}
}