using Microsoft.EntityFrameworkCore;
using CultureStay.Infrastructure.Data;

namespace CultureStay.Extensions;

public static class MigrateExtension
{
    public static async Task MigrateDatabaseAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;
        var dbContext = services.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync();
    } 
}
