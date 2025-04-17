using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Plainer.Data;
using Plainer.DTOs.RoleDTO;
using Plainer.Entities;
using Plainer.Helper;

namespace Plainer.Endpoints;

public static class PlainerEventParticipantsEndpoints
{
    public static WebApplication MapPlainerEventParticipantsEndpoints(this WebApplication app){

    app.MapPost("events/{eventId}/participants/{targetUserId}/role", [Authorize] async (HttpContext ctx,PlainerDbContext dbContext,int eventId,int targetUserId,RoleUpdateDTO newRole) =>
        {
            var currentUserId = int.Parse(ctx.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var currentParticipant = await dbContext.EventParticipants
                .Include(p => p.Role)
                .FirstOrDefaultAsync(x => x.UserId == currentUserId && x.EventId == eventId);

            var targetParticipant = await dbContext.EventParticipants
                .Include(p => p.Role)
                .FirstOrDefaultAsync(x => x.UserId == targetUserId && x.EventId == eventId);

            if(EventParticipantsHelper.EventParticipantChecks(currentParticipant,targetParticipant) == false){
                return Results.Forbid();
            };

            if (newRole.NewRoleId <= currentParticipant!.RoleId)
            {
                return Results.Forbid();
            }

            targetParticipant!.RoleId = newRole.NewRoleId;

            await dbContext.SaveChangesAsync();

            return Results.NoContent();
            }
        );

        //Adding user to event
        app.MapPost("events/{eventId}/participants/{targetUserUsername}/", [Authorize] async (HttpContext ctx, PlainerDbContext dbContext, int eventId, string targetUserUsername) =>
            {
                var currentUserId = int.Parse(ctx.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                var currentParticipant = await dbContext.EventParticipants
                .Include(p => p.Role)
                .FirstOrDefaultAsync(x => x.UserId == currentUserId && x.EventId == eventId);

                var targetParticipant = await dbContext.Users.FirstOrDefaultAsync(X=>X.Username == targetUserUsername);

                if (currentParticipant is null || targetParticipant is null)
                {
                    return Results.NotFound("Target user doesn't exist");
                }

                if (currentParticipant.RoleId > 2)
                {
                    return Results.Forbid();
                }

                var newEventParticipant = new EventParticipant{
                    UserId = targetParticipant.Id,
                    EventId = eventId,
                    RoleId = 3
                };

                await dbContext.EventParticipants.AddAsync(newEventParticipant);
                await dbContext.SaveChangesAsync();

                return Results.NoContent();
            }
        );

        //Removing User from the event
        app.MapDelete("events/{eventId}/participants/{targetUserId}/", [Authorize] async (HttpContext ctx, PlainerDbContext dbContext, int eventId, int targetUserId) =>
            {
                var currentUserId = int.Parse(ctx.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                var currentParticipant = await dbContext.EventParticipants.Include(p => p.Role).FirstOrDefaultAsync(X=>X.UserId == currentUserId);

                var targetParticipant = await dbContext.EventParticipants.Include(p => p.Role).FirstOrDefaultAsync(X=>X.UserId == targetUserId && X.EventId == eventId);

                if(!EventParticipantsHelper.EventParticipantChecks(currentParticipant,targetParticipant)){
                    return Results.Forbid();
                };

                await dbContext.EventParticipants.Where(x=>x.UserId == targetParticipant!.UserId && x.EventId == eventId).ExecuteDeleteAsync();
                await dbContext.SaveChangesAsync();

                return Results.Ok();
            }
        );

        return app;
    }
}