using CultureStay.Application.Services.Interface;
using CultureStay.Application.ViewModels.PaymentInfo.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CultureStay.Controllers;

[Route("api/payment-info")]
[ApiController]
[Authorize]
public class PaymentInfoController : ControllerBase
{
    private readonly IPaymentInfoService _paymentInfoService;
    public PaymentInfoController(IPaymentInfoService paymentInfoService)
    {
        _paymentInfoService = paymentInfoService;
    }
    
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _paymentInfoService.GetPaymentInfoAsync();
        return Ok(result);
    }
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PaymentInfoResponse paymentInfoDto)
    {
        var result = await _paymentInfoService.CreatePaymentInfoAsync(paymentInfoDto);
        return Ok(result);
    }

    [HttpPut()]
    public async Task<IActionResult> Update([FromBody] PaymentInfoResponse paymentInfoDto)
    {
        var result = await _paymentInfoService.UpdatePaymentInfoAsync(paymentInfoDto);
        return Ok(result);
    }

}