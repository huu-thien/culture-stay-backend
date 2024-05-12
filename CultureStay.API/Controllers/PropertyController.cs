using CultureStay.Application.Services.Interface;
using CultureStay.Application.ViewModels.PropertyUtility.Response;
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
        return Ok(result);
    }

    [HttpGet("host/{hostId:int}")]
    public async Task<IActionResult> GetPropertyListByHostId(int hostId, [FromQuery] PropertyQueryParameters pqp)
    {
        var result = await _propertyService.GetListPropertyByHostIdAsync(hostId, pqp);
        return Ok(result);
    }
}