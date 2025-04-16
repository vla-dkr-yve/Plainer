using System;
using Microsoft.EntityFrameworkCore;
using Plainer.Data;

namespace Plainer.Endpoints;

public static class PlainerRoleEndpoints
{
    public static RouteGroupBuilder MapPlainerRoleEndpoints(this WebApplication app){
        var group = app.MapGroup("roles");

        group.MapGet("/", async (PlainerDbContext dbContext) => {
                return await dbContext.Roles.ToListAsync();
            }
        );

        return group;
    }
}
