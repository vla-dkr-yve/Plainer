using Microsoft.EntityFrameworkCore;

namespace Plainer.Data;

public static class DataExtensions
{
    public static void MigrateDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PlainerDbContext>();
        dbContext.Database.Migrate();
    }
}
