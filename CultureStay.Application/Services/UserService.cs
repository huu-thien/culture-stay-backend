using AutoMapper;
using CultureStay.Application.Common.Interfaces;
using CultureStay.Application.Common.Models;
using CultureStay.Application.Services.Interface;
using CultureStay.Application.ViewModels.User.Request;
using CultureStay.Application.ViewModels.User.Response;
using CultureStay.Domain.Entities;
using CultureStay.Domain.Exceptions;
using CultureStay.Domain.Repositories.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CultureStay.Application.Services;

public class UserService(UserManager<User> userManager, 
    IRepositoryBase<Host> hostRepository, 
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork, IMapper mapper) : BaseService(unitOfWork, mapper, currentUser), IUserService
{
    public async Task<GetUsersForAdminResponse> GetUserByIdAsync(int id)
    {
        var user = await userManager.Users
            .Include(u => u.Host)
            .Include(u => u.Guest)
            .FirstOrDefaultAsync(u => u.Id == id);
        if (user is null) throw new EntityNotFoundException(nameof(User), id.ToString());

        var result = Mapper.Map<GetUsersForAdminResponse>(user);
        result.IsHost = user.Host is not null;
        return result;
    }

    public async Task<PaginatedList<GetUsersForAdminResponse>> GetUsersForAdminAsync(UserPagingParameters pp)
    {
        var skip = (pp.PageIndex - 1) * pp.PageSize;
        var take = pp.PageSize;
        
        var adminIds = (await userManager.GetUsersInRoleAsync("Admin")).Select(x => x.Id);
        
        var users = await userManager.Users
            .Include(u => u.Host)
            .Include(u => u.Guest)
            .Where(u => adminIds.All(id => id != u.Id))
            .Where(u => !pp.IsHostOnly || u.Host != null)
            .OrderBy(i => pp.IsDescending ? -i.Id : i.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
        
        var totalCount = await userManager.Users
            .Where(u => adminIds.All(id => id != u.Id))
            .Where(u => !pp.IsHostOnly || u.Host != null)
            .CountAsync();

        var result = Mapper.Map<List<GetUsersForAdminResponse>>(users);
        foreach (var user in result)
        {
            user.IsHost = users.FirstOrDefault(u => u.Id == user.Id)?.Host is not null;
        }
        
        return new PaginatedList<GetUsersForAdminResponse>(result, totalCount, pp.PageIndex, pp.PageSize);
    }

    public async Task<GetUsersForAdminResponse> UpdateUserAsync(int id, UpdateUserInfoRequest request)
    {
        var user = await userManager.Users
            .Include(u => u.Host)
            .Include(u => u.Guest)
            .FirstOrDefaultAsync(u => u.Id == id);
        if (user is null) throw new EntityNotFoundException(nameof(User), id.ToString());
        
        Mapper.Map(request, user);
        await unitOfWork.SaveChangesAsync();
        
        var result = Mapper.Map<GetUsersForAdminResponse>(user);
        result.IsHost = user.Host is not null;
        return result;
    }
}
