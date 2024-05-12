using System.Net;
using CultureStay.Application.ViewModels.Auth.Requests;
using CultureStay.Application.ViewModels.Auth.Responses;
using CultureStay.Domain.Entities;
using Google.Apis.Oauth2.v2.Data;

namespace CultureStay.Application.Services.Interface;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<HttpStatusCode> RegisterAsync(RegisterRequest request);
    Task<RefreshTokenResponse> RefreshTokenAsync(string refreshToken);
    Task<LoginResponse> GoogleAuthenticateAsync(string accessToken);
    Task<User> GetOrCreateUserAsync(Userinfo payload);

}