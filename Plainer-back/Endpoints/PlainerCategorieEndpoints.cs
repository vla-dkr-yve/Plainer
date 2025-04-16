using Microsoft.EntityFrameworkCore;
using Plainer.Data;

namespace Plainer.Endpoints;

public static class PlainerCategoriesEndpoints
{
    public static RouteGroupBuilder MapPlainerCategoriesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("categories");

        group.MapGet("/", async (PlainerDbContext dbContext) => {
            var categories = await dbContext.Categories.Select(x => x.Name).ToListAsync();

            return Results.Ok(categories);
        });
        return group;
    }
}
