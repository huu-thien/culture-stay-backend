using CultureStay.Application.Services.Interface;
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
        return Ok(result);
    }
    
    /// <summary>
    ///  Host review Guest: Check xem guest đã từng ở tại property của host hay chưa (nếu mình không phải host hoặc chưa login thì luôn false)
    /// </summary>
    [HttpGet("{guestId:int}/is-stayed")]
    [Authorize]
    public async Task<IActionResult> CheckGuestIsStayed(int guestId)
    {
        var result = await _guestService.CheckGuestIsStayedAsync(guestId);
        return Ok(result);
    }
}