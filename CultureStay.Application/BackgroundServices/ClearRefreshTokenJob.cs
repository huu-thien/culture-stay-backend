using Microsoft.EntityFrameworkCore;
using Quartz;
using CultureStay.Domain.Entities;
using CultureStay.Domain.Repositories.Base;

namespace CultureStay.Application.BackgroundServices;

public class ClearRefreshTokenJob : IJob
{
	private readonly IRepositoryBase<RefreshToken> _refreshTokenRepository;

	public ClearRefreshTokenJob(IRepositoryBase<RefreshToken> refreshTokenRepository)
	{
		_refreshTokenRepository = refreshTokenRepository;
	}

	public async Task Execute(IJobExecutionContext context)
	{
		await _refreshTokenRepository.GetQuery(r => r.Expires < DateTime.UtcNow.AddDays(-1)).ExecuteDeleteAsync();
	}
}