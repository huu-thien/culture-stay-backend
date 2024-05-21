using CultureStay.Application.Services.Interface;
using CultureStay.Application.ViewModels;
using CultureStay.Application.ViewModels.Guest.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CultureStay.Controllers;

[ApiController]
[Route("api/guests")]
public class GuestController : ControllerBase
{
    private readonly IGuestService _guestService;
    public GuestController(IGuestService guestService)
    {
        _guestService = guestService;
    }
    
    [HttpGet("{guestId:int}")]
    public async Task<IActionResult> GetGuestById(int guestId)
    {
        var result = await _guestService.GetGuestByIdAsync(guestId);
        return Ok(new BaseResponse<GetGuestResponse>{Message = "Get guest successfully", Data = result});
    }
    
    /// <summary>
    ///  Host review Guest: Check xem guest đã từng ở tại property của host hay chưa (nếu mình không phải host hoặc chưa login thì luôn false)
    /// </summary>
    [HttpGet("{guestId:int}/is-stayed")]
    [Authorize]
    public async Task<IActionResult> CheckGuestIsStayed(int guestId)
    {
        var result = await _guestService.CheckGuestIsStayedAsync(guestId);
        return Ok(new BaseResponse<bool>{Message = "Check guest is stayed successfully", Data = result});
    }
}