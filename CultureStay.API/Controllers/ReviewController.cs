﻿
using CultureStay.Application.Services.Interface;
using CultureStay.Application.ViewModels.Review.QueryParameters;
using CultureStay.Application.ViewModels.Review.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CultureStay.Controllers;

[ApiController]
[Route("api/reviews")]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;
    
    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }
    
    // Property Review
    [HttpGet("property/{propertyId}")]
    public async Task<IActionResult> GetPropertyReviews(int propertyId, [FromQuery] ReviewQueryParameters rqp)
    {
        var result = await _reviewService.GetListPropertyReviewAsync(propertyId, rqp);
        return Ok(result);
    }
    
    [Authorize]
    [HttpPost("property/{propertyId}")]
    public async Task<IActionResult> CreatePropertyReview(int propertyId, CreatePropertyReviewRequest request)
    {
        var result = await _reviewService.CreatePropertyReviewAsync(propertyId, request);
        return Ok(result);
    }
    
    // [HttpPut("property/{propertyId}/review/{reviewId}")]
    
    /// <summary>
    /// Only admin or creator of the review can delete the review
    /// </summary>
    [Authorize]
    [HttpDelete("property/{reviewId}")]
    public async Task<IActionResult> DeletePropertyReview(int reviewId)
    {
        await _reviewService.DeletePropertyReviewAsync(reviewId);
        return NoContent();
    }
}