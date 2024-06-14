using CultureStay.Application.Services.Interface;
using CultureStay.Application.ViewModels;
using CultureStay.Application.ViewModels.Payment.QueryParameters;
using CultureStay.Application.ViewModels.Payment.Request;
using CultureStay.Application.ViewModels.Payment.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CultureStay.Controllers;


[Route("api/payment")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }
    [HttpPost("create-payment")]
    public async Task<IActionResult> CreatePayment([FromBody] CreateBookingPaymentRequest createPaymentRequest)
    {
        var ip = HttpContext.Connection.RemoteIpAddress!.ToString();
        string url = await _paymentService.CreateBookingPayment(ip, createPaymentRequest);
        return Ok(new BaseResponse<string>{Message = "Create payment success", Data = url});
    }

    [HttpPost("vnpay-hook-url")]
    public async Task<IActionResult> ReceiveDataFromVnp(VnPayReturnDto vnpayData)
    {
        await _paymentService.ReceiveDataFromVnp(vnpayData);
        return Ok(vnpayData);
    }
        
    [HttpGet("vnpay-hook-url")]
    public async Task<IActionResult> ReceiveDataFromVnpGet([FromQuery] VnPayReturnDto vnpayData)
    {
        await _paymentService.ReceiveDataFromVnp(vnpayData);
        return Ok(vnpayData);
    }

    [HttpGet("charge-Payment")]
    [Authorize]
    public async Task<IActionResult> ChargePayment([FromQuery] ChargePaymentQueryParameter pqp)
    {
        var result = await _paymentService.ChargePayment(pqp);
        return Ok(result);
    }

    [HttpGet("refund-Payment")]
    [Authorize]
    public async Task<IActionResult> RefundPayment([FromQuery] RefundPaymentQueryParameter pqp)
    {
        var result= await _paymentService.RefundPayment(pqp);
        return Ok(result);
    }
}