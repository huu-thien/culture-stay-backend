using System.Text.Json.Serialization;
using FluentValidation;
using Serilog;
using CultureStay.Application.Services;
using CultureStay.Chat;
using CultureStay.Extensions;
using CultureStay.Middlewares;
using Minio;
using IRequest = CultureStay.Application.ViewModels.IRequest;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureLogging();
builder.AddSettings();
builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services
    .ConfigureCors()
    .ConfigureDbContext(builder.Configuration)
    .ConfigureIdentity()
    .AddSwagger("CultureStay API v1")
    .AddRepositories()
    .AddServices()
    .AddMinio(builder.Configuration)
    .AddCurrentUser()
    .AddQuartz()
    .AddEmailSender()
    .AddAutoMapper(typeof(BaseService).Assembly)
    .AddProblemDetails()
    .AddExceptionHandler<GlobalExceptionHandler>()
    .AddAuthentication(builder.Configuration)
    .AddValidatorsFromAssemblyContaining<IRequest>(ServiceLifetime.Singleton)
    .AddHttpClient()
    .AddSignalR(options => 
    { 
        options.EnableDetailedErrors = true; 
    });

var app = builder.Build();

await app.MigrateDatabaseAsync();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
    app.UseSwaggerUI(c => { c.ConfigObject.AdditionalItems.Add("persistAuthorization", "true"); });
}

app.UseSerilogRequestLogging();
app.UseExceptionHandler();
app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();
// app.UseHttpsRedirection();

app.MapHub<ChatHub>("/chathub");
app.MapControllers();

app.Run();
