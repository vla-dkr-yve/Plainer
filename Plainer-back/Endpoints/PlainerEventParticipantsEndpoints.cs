using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Plainer.Data;
using Plainer.DTOs.RoleDTO;

namespace Plainer.Endpoints;

public static class PlainerEventParticipantsEndpoints
{
    public static WebApplication MapPlainerEventParticipantsEndpoints(this WebApplication app){
        //var group = app.MapGroup("events/{eventId}");

    app.MapPut("events/{eventId}/participants/{targetUserId}/role", [Authorize] async (HttpContext ctx,PlainerDbContext dbContext,int eventId,int targetUserId,RoleUpdateDTO newRole) =>
        {
        var currentUserId = int.Parse(ctx.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        // Get current participant in THIS event
        var currentParticipant = await dbContext.EventParticipants
            .Include(p => p.Role)
            .FirstOrDefaultAsync(x => x.UserId == currentUserId && x.EventId == eventId);

        if (currentParticipant is null)
        {
            return Results.Forbid();
        }

        var currentPower = currentParticipant.RoleId;

        // Get target participant in THIS event
        var targetParticipant = await dbContext.EventParticipants
            .Include(p => p.Role)
            .FirstOrDefaultAsync(x => x.UserId == targetUserId && x.EventId == eventId);

        if (targetParticipant is null)
        {
            return Results.NotFound("Target is not found");
        }

        var targetPower = targetParticipant.RoleId;

        if (currentPower >= targetPower)
        {
            return Results.Forbid(); // Can't change role of higher/equal one
        }

        if (newRole.NewRoleId < currentPower)
        {
            return Results.Forbid(); // Can't assign a stronger role
        }

        targetParticipant.RoleId = newRole.NewRoleId;

        await dbContext.SaveChangesAsync();

        return Results.NoContent();
        });


        return app;
    }
}
