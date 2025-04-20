using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Plainer.Data;
using Plainer.DTOs.EventDTOs;
using Plainer.Entities;
using Plainer.Mapping;

namespace Plainer.Endpoints;

public static class PlainerEventEndpoints
{
    public static RouteGroupBuilder MapPlainerEventEndpoints(this WebApplication app){

        var group = app.MapGroup("events");

        //Get Events/
        group.MapGet("/", [Authorize] async (HttpContext ctx, PlainerDbContext dbContext) => 
            {
                var userId = int.Parse(ctx.User.FindFirstValue(ClaimTypes.NameIdentifier)!);


                var events = await dbContext.Events
                .Include(x => x.Category)
                .Include(x => x.User)
                .Include(x => x.EventParticipants)
                .Where(x => x.CreatedBy == userId || x.EventParticipants.Any(ep=>ep.UserId == userId))
                .Select(x => x.ToDTO())
                .ToListAsync();

                return Results.Ok(events);
            }
        );

        //Get Events/id
        group.MapGet("/{id}", [Authorize] async (int id, PlainerDbContext dbContext) => 
            {
                Event? res = await dbContext.Events
                .Include(x => x.Category)
                .Include(x => x.User)
                .Include(x => x.EventParticipants)
                    .ThenInclude(ep=>ep.User)
                .Include(x => x.EventParticipants)
                    .ThenInclude(ep=>ep.Role)
                .FirstOrDefaultAsync(x => x.Id == id);

                return res is null ? Results.NotFound() : Results.Ok(res.ToDetailedDTO());
            }
        );

        //Post Event
        group.MapPost("/", [Authorize] async (HttpContext ctx, CreateEventDTO newEvent, PlainerDbContext dbContext) =>
            {
                var userId = int.Parse(ctx.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var role = await dbContext.Roles.FindAsync(1);

                var @event = new Event
                {
                    Title = newEvent.Title,
                    Description = newEvent.Description,
                    StartTime = newEvent.StartTime,
                    EndTime = newEvent.EndTime,
                    CategoryId = newEvent.CategoryId,
                    CreatedBy = userId,
                    CreatedAt = DateTime.Now,
                    IsChecked = false,
                    EventParticipants = new List<EventParticipant>()
                    {
                        new EventParticipant
                        {
                            UserId = userId,
                            RoleId = 1,
                        }
                    }
                };

                dbContext.Events.Add(@event);
                await dbContext.SaveChangesAsync();

                return Results.Created($"/{@event.Id}", @event.ToDTO());
            }
        );

            //Put Event
        group.MapPut("/{id}", [Authorize] async (int id, UpdateEventDTO updatedEvent, PlainerDbContext dbContext) => 
            {
                Event? oldEvent = await dbContext.Events.Include(x => x.EventParticipants).Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == id);

                if(oldEvent is null){
                    return Results.NoContent();
                }  

                oldEvent.Title = updatedEvent.Title;
                oldEvent.Description = updatedEvent.Description;
                oldEvent.StartTime = updatedEvent.StartTime;
                oldEvent.EndTime = updatedEvent.EndTime;
                oldEvent.CategoryId = updatedEvent.CategoryId;
                oldEvent.IsChecked = updatedEvent.IsChecked;

                dbContext.EventParticipants.RemoveRange(oldEvent.EventParticipants);

                foreach(var participant in updatedEvent.EventParticipants){
                    oldEvent.EventParticipants.Add(new EventParticipant{
                            UserId = participant.UserId,
                            RoleId = participant.RoleId,
                            EventId = oldEvent.Id
                        }
                    );  
                }

                await dbContext.SaveChangesAsync();

                return Results.NoContent();
            }
        );

        //Delete Event
        group.MapDelete("/{id}", [Authorize] async (int id, PlainerDbContext dbContext) =>
            {
                await dbContext.Events.Where(x=>x.Id == id).ExecuteDeleteAsync();

                await dbContext.SaveChangesAsync();
                return Results.NoContent();
            }
        );

        return group;
    }
}
