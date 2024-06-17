using System.Net;
using AutoMapper;
using CultureStay.Application.Common.Interfaces;
using CultureStay.Application.Common.Specifications;
using CultureStay.Application.Services.Interface;
using CultureStay.Application.ViewModels.Auth.Requests;
using CultureStay.Application.ViewModels.Auth.Responses;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Oauth2.v2;
using Google.Apis.Oauth2.v2.Data;
using Google.Apis.Services;
using Microsoft.AspNetCore.Identity;
using CultureStay.Domain;
using CultureStay.Domain.Entities;
using CultureStay.Domain.Exceptions;
using CultureStay.Domain.Repositories.Base;
using CultureStay.Domain.Resources;

namespace CultureStay.Application.Services;

public class AuthService(
	ITokenService tokenService,
	UserManager<User> userManager,
	IRepositoryBase<Guest> guestRepository,	
	IRepositoryBase<Host> hostRepository,
 	IUnitOfWork unitOfWork,
	IMapper mapper,
	ICurrentUser currentUser) : BaseService(unitOfWork, mapper, currentUser), IAuthService
{
	public async Task<LoginResponse> LoginAsync(LoginRequest request)
	{
		User? user;
		if (request.Identifier.Contains("@"))
			user = await userManager.FindByEmailAsync(request.Identifier);
		else
			user = await userManager.FindByNameAsync(request.Identifier);

		if (user is null)
			throw new AuthException();

		var isPasswordValid = await userManager.CheckPasswordAsync(user, request.Password);
		AuthException.ThrowIfFalse(isPasswordValid);

		var token = tokenService.GenerateToken(user);
		var (refreshToken, expires) = tokenService.GenerateRefreshToken();

		await tokenService.SaveRefreshTokenAsync(refreshToken, user.Id, expires);

		var userResponse = Mapper.Map<GetUserResponse>(user);
		userResponse.IsHost = await hostRepository.AnyAsync(new HostByUserIdSpecification(user.Id));
		var role = userManager.GetRolesAsync(user).Result.First();

		return new LoginResponse(token, refreshToken, userResponse, role);
	}

	public async Task<HttpStatusCode> RegisterAsync(RegisterRequest request)
	{
		var isEmailTaken = await userManager.FindByEmailAsync(request.Email) != null;
		if (isEmailTaken)
			throw new AlreadyExistsException(nameof(User.Email), request.Email);

		var isUserNameTaken = await userManager.FindByNameAsync(request.UserName) != null;
		if (isUserNameTaken)
			throw new AlreadyExistsException(nameof(User.UserName), request.UserName);

		var user = new User
		{
			UserName = request.UserName,
			Email = request.Email,
			FullName = request.FullName
		};
		

		await UnitOfWork.BeginTransactionAsync();

		try
		{
			var result = await userManager.CreateAsync(user, request.Password);
			if (result.Succeeded)
			{
				result = await userManager.AddToRoleAsync(user, AppRole.User.ToValue());
				if (result.Succeeded)
				{
					guestRepository.Add(new Guest { UserId = user.Id });
					await unitOfWork.SaveChangesAsync();
					await UnitOfWork.CommitTransactionAsync();
					return HttpStatusCode.OK; // Return status 200 if registration is successful
				}
			}
			throw new AppException(result.Errors.First().Description);
		}
		catch
		{
			await UnitOfWork.RollbackAsync();
			throw;
		}
	}

	public async Task<RefreshTokenResponse> RefreshTokenAsync(string refreshToken)
	{
		var refreshTokenEntity = await tokenService.ValidateRefreshTokenAsync(refreshToken);

		if (refreshTokenEntity.IsExpired)
			throw new AppException(ErrorMessages.RefreshTokenExpired);

		var user = await userManager.FindByIdAsync(refreshTokenEntity.UserId.ToString())
		           ?? throw new AuthException();

		var (newRefreshToken, expires) = tokenService.GenerateRefreshToken();

		var tokenDto = new RefreshTokenResponse(tokenService.GenerateToken(user), newRefreshToken);

		refreshTokenEntity.Token = newRefreshToken;
		refreshTokenEntity.Expires = expires;

		await UnitOfWork.SaveChangesAsync();

		return tokenDto;
	}

	public async Task<LoginResponse> GoogleAuthenticateAsync(string accessToken)
	{
		// get user info from access token
		var userInfo = await new Oauth2Service(new BaseClientService.Initializer
		{
			HttpClientInitializer = GoogleCredential.FromAccessToken(accessToken),
			ApplicationName = "CultureStay"
		}).Userinfo.Get().ExecuteAsync();

		var user = await GetOrCreateUserAsync(userInfo);
		
		var guest = await guestRepository.FindOneAsync(new GuestByUserIdSpecification(user.Id));
		if (guest == null)
		{
			guestRepository.Add(new Guest { UserId = user.Id });
			await unitOfWork.SaveChangesAsync();
		}
		
		var userDto = Mapper.Map<GetUserResponse>(user);
		userDto.IsHost = await hostRepository.AnyAsync(new HostByUserIdSpecification(user.Id));
		
		var token = tokenService.GenerateToken(user);
		var (refreshToken, expires) = tokenService.GenerateRefreshToken();

		await tokenService.SaveRefreshTokenAsync(refreshToken, user.Id, expires);
		
		var userResponse = Mapper.Map<GetUserResponse>(user);
		var role = AppRole.User.ToValue();

		return new LoginResponse(token, refreshToken, userResponse, role);
	}

	public async Task<User> GetOrCreateUserAsync(Userinfo payload)
	{
		var user = await userManager.FindByEmailAsync(payload.Email);
		if (user != null) return user;

		user = new User
		{
			Email = payload.Email,
			UserName = payload.Email,
			FullName = payload.Name,
			EmailConfirmed = true,
			AvatarUrl = payload.Picture
		};

		await UnitOfWork.BeginTransactionAsync();

		try
		{
			var result = await userManager.CreateAsync(user);
			if (!result.Succeeded)
				throw new AppException(result.Errors.First().Description);

			var addRoleResult = await userManager.AddToRoleAsync(user, AppRole.User.ToValue());
			if (!addRoleResult.Succeeded)
				throw new AppException(result.Errors.First().Description);

			await UnitOfWork.SaveChangesAsync();

			await UnitOfWork.CommitTransactionAsync();
		}
		catch
		{
			await UnitOfWork.RollbackAsync();
			throw;
		}

		return user;
	}
}