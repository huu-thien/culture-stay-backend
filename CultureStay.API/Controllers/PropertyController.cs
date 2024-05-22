using CultureStay.Application.Services.Interface;
using CultureStay.Application.ViewModels;
using CultureStay.Application.ViewModels.Property.Request;
using CultureStay.Application.ViewModels.Property.Response;
using CultureStay.Application.ViewModels.PropertyUtility.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CultureStay.Controllers;


[ApiController]
[Route("api/properties")]
public class PropertyController : ControllerBase
{
    private readonly IPropertyService _propertyService;
    
    public PropertyController(IPropertyService propertyService)
    {
        _propertyService = propertyService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetPropertyList([FromQuery] PropertyQueryParameters pqp)
    {
        var result = await _propertyService.GetListPropertyAsync(pqp);
        return Ok(result);
    }
    
    [HttpGet("{propertyId:int}")]
    public async Task<IActionResult> GetPropertyById(int propertyId)
    {
        var result = await _propertyService.GetPropertyByIdAsync(propertyId);
        return Ok(new BaseResponse<GetPropertyResponse>{Message = "Get property successfully", Data = result});
    }

    [HttpGet("host/{hostId:int}")]
    public async Task<IActionResult> GetPropertyListByHostId(int hostId, [FromQuery] PropertyQueryParameters pqp)
    {
        var result = await _propertyService.GetListPropertyByHostIdAsync(hostId, pqp);
        return Ok(result);
    }
    
    [HttpGet("{propertyId:int}/is-stayed")]
    public async Task<IActionResult> IsStayed(int propertyId)
    {
        var result = await _propertyService.IsStayedAsync(propertyId);
        return Ok(new BaseResponse<bool>{Message = "Check stayed successfully", Data = result});
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateProperty([FromBody] CreatePropertyRequest request)
    {
        var result = await _propertyService.CreatePropertyAsync(request);
        return Ok(new BaseResponse<GetPropertyResponse>{Message = "Create property successfully", Data = result});
    }
    
    [Authorize]
    [HttpPut("{propertyId:int}")]
    public async Task<IActionResult> UpdateProperty(int propertyId, [FromBody] CreatePropertyRequest request)
    {
        var result = await _propertyService.UpdatePropertyAsync(propertyId, request);
        return Ok(new BaseResponse<GetPropertyResponse>{Message = "Update property successfully", Data = result});
    }
    
    [Authorize]
    [HttpDelete("{propertyId:int}")]
    public async Task<IActionResult> DeleteProperty(int propertyId)
    {
        await _propertyService.DeletePropertyAsync(propertyId);
        return Ok(new BaseResponse<string>{Message = "Delete property successfully"});
    }
}