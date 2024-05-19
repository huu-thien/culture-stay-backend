using CultureStay.Application.Services.Interface;
using CultureStay.Application.ViewModels;
using CultureStay.Application.ViewModels.User.Request;
using CultureStay.Application.ViewModels.User.Response;
using Microsoft.AspNetCore.Mvc;

namespace CultureStay.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    
    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var result = await _userService.GetUserByIdAsync(id);
        return Ok(new BaseResponse<GetUsersForAdminResponse>{Message = "Get User Success", Data = result});
    }
    
    [HttpGet]
    public async Task<IActionResult> GetUsersForAdmin([FromQuery] UserPagingParameters pp)
    {
        var result = await _userService.GetUsersForAdminAsync(pp);
        return Ok(result);
    }
    
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateUser(int id, UpdateUserInfoRequest request)
    {
        var result = await _userService.UpdateUserAsync(id, request);
        return Ok(new BaseResponse<GetUsersForAdminResponse>{Message = "Update User Success", Data = result});
    }
}