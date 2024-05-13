using CultureStay.Application.Common;
using CultureStay.Application.Common.Models;
using CultureStay.Application.Services.Interface;
using CultureStay.Application.ViewModels;
using CultureStay.Application.ViewModels.Property.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CultureStay.Controllers;


[Authorize]
[ApiController]
[Route("api/wishlists")]
public class WishlistController : ControllerBase
{
    private readonly IWishlistService _wishlistService;
    
    public WishlistController(IWishlistService wishlistService)
    {
        _wishlistService = wishlistService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetWishListPropertyList([FromQuery] PagingParameters pqp)
    {
        var result = await _wishlistService.GetListWishlistAsync(pqp);
        return Ok(new BaseResponse<PaginatedList<GetPropertyResponse>>{Data = result, Message = "Get list wishlist success"});
    }
    
    [HttpPost("properties/{propertyId:int}")]
    public async Task<IActionResult> AddWishlistProperty(int propertyId)
    {
        await _wishlistService.AddWishLlistAsync(propertyId);
        return Ok(new BaseResponse {Message = "Add wishlist success"});
    }
    
    [HttpDelete("properties/{propertyId:int}")]
    public async Task<IActionResult> DeleteWishlistProperty(int propertyId)
    {
        await _wishlistService.RemoveWishlistAsync(propertyId);
        return Ok(new BaseResponse{Message = "Delete wishlist success"});
    }
}