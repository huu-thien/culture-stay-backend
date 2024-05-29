using CultureStay.Application.Common;
using CultureStay.Application.Services.Interface;
using CultureStay.Application.ViewModels;
using CultureStay.Application.ViewModels.Host.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CultureStay.Controllers;


[ApiController]
[Route("api/hosts")]
public class HostController : ControllerBase
{
    private readonly IHostService _hostService;
    public HostController(IHostService hostService)
    {
        _hostService = hostService;
    }
    
    [HttpGet("{hostId:int}")]
    public async Task<IActionResult> GetHostAsync(int hostId)
    {
        var host = await _hostService.GetHostByIdAsync(hostId);
        return Ok(new BaseResponse<GetHostResponse>{Message = "Get host successfully", Data = host});
    }
    
    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetHostByUserIdAsync(int userId)
    {
        var host = await _hostService.GetHostByUserIdAsync(userId);
        return Ok(new BaseResponse<GetHostResponse>{Message = "Get host successfully", Data = host});
    }
    
    [HttpGet]
    public async Task<IActionResult> GetHostsForAdminAsync([FromQuery] PagingParameters pp)
    {
        var hosts = await _hostService.GetHostsForAdminAsync(pp);
        return Ok(hosts);
    }
    
    [HttpGet("{hostId:int}/is-stayed")]
    [Authorize]
    public async Task<IActionResult> CheckGuestStayedInPropertyOfHost(int hostId)
    {
        var result = await _hostService.CheckHostIsStayedAsync(hostId);
        return Ok(new BaseResponse<bool>{Message = "Check host rented guest yet", Data = result});
    }
}