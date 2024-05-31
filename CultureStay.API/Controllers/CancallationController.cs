using CultureStay.Application.Services.Interface;
using CultureStay.Application.ViewModels;
using CultureStay.Application.ViewModels.Cancellation.QueryParameters;
using CultureStay.Application.ViewModels.Cancellation.Request;
using CultureStay.Application.ViewModels.Cancellation.Response;
using CultureStay.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CultureStay.Controllers;

[ApiController]
[Route("api/cancellations")]
[Authorize]
public class CancallationController : ControllerBase
{
    private readonly ICancellationService _cancellationService;
    public CancallationController(ICancellationService cancellationService)
    {
        _cancellationService = cancellationService;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateCancellationTicket([FromBody] CreateCancellationRequest? request)
    {
        if (request is null)
            return BadRequest();
        await _cancellationService.CreateCancellationTicketAsync(request);
        return Ok(new BaseResponse(){Message = "Create cancellation ticket successfully"});
    }
    
    [HttpGet]
    public async Task<IActionResult> GetCancellationTickets([FromQuery] CancellationTicketQueryParameters ctp)
    {
        var result = await _cancellationService.GetCancellationTicketAsync(ctp);
        return Ok(result);
    }
    
    [HttpGet("{cancellationTicketId:int}")]
    public async Task<IActionResult> GetCancellationTicketById(int cancellationTicketId)
    {
        var result = await _cancellationService.GetCancellationTicketByIdAsync(cancellationTicketId);
        return Ok(new BaseResponse<GetCancellationResponse>{Message = "Get cancellation ticket successfully", Data = result});
    }
    
    [Authorize(Roles = AppRole.Admin)]
    [HttpPost("{cancellationTicketId:int}/reject")]
    public async Task<IActionResult> RejectCancellationTicket(int cancellationTicketId, [FromBody] RejectCancellationTicketRequest? request)
    {
        if (request is null)
            return BadRequest();
        await _cancellationService.RejectCancellationTicketAsync(cancellationTicketId, request);
        return Ok(new BaseResponse(){Message = "Reject cancellation ticket successfully"});
    }
    
    [Authorize(Roles = AppRole.Admin)]
    [HttpPost("{cancellationTicketId:int}/accept")]
    public async Task<IActionResult> AcceptCancellationTicket(int cancellationTicketId, [FromBody] string? resolveNote)
    {
        await _cancellationService.AcceptCancellationTicketAsync(cancellationTicketId, resolveNote);
        return Ok(new BaseResponse(){Message = "Accept cancellation ticket successfully"});
    }
}