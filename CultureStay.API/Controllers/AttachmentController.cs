using CultureStay.Application.Common.Configurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace CultureStay.Controllers;


[ApiController]
[Route("api/attachments")]
public class AttachmentController : ControllerBase
{
    private readonly IMinioClient _minioClient;
    private readonly MinioSettings _minioSettings;

    public AttachmentController(IMinioClient minioClient, IOptions<MinioSettings> minioSettings)
    {
        _minioClient = minioClient;
        _minioSettings = minioSettings.Value;
    }
    [HttpPost("upload-attachment")]
    public async Task<IActionResult> TestMinio([FromForm] IFormFile file)
    {
        if (!await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_minioSettings.BucketName)))
            return BadRequest("Bucket does not exist");
        using var newMemoryStream = new MemoryStream();
        
        var fileName = file.FileName;
        
        await file.CopyToAsync(newMemoryStream);
        
        var contentType = file.ContentType;
        
        var size = newMemoryStream.Length;
        newMemoryStream.Position = 0;
        var args = new PutObjectArgs()
            .WithBucket(_minioSettings.BucketName)
            .WithObject($"/{fileName}")
            .WithObjectSize(size)
            .WithContentType(contentType)
            .WithStreamData(newMemoryStream);
        
        await _minioClient.PutObjectAsync(args);

        return Ok($"{_minioSettings.BaseUrl}/{fileName}");
    }
}