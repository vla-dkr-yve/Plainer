using Plainer.DTOs.EventDTOs;
using Plainer.Entities;

namespace Plainer.Mapping;

public static class EventMapping
{
    public static EventDTO ToDTO(this Event @event){
        int NumberOfParticipants = @event.EventParticipants.Count;
        return new(
            @event.Id,
            @event.Title,
            @event.Description,
            @event.StartTime,
            @event.EndTime,
            @event.CategoryId,
            @event.CreatedBy,
            @event.CreatedAt,
            NumberOfParticipants,
            @event.IsChecked
        );
    }

    public static EventDetailedDTO ToDetailedDTO(this Event @event){
        var EventParticipants = new List<EventParticipantDetailedDTO>();

        foreach(var participant in @event.EventParticipants){
            EventParticipants.Add(new(
                participant.UserId,
                participant.User.Username,
                participant.Role.Id,
                participant.Role.Name
            ));
        }

        string category;

        if (@event.CategoryId == null)
        {
            category = "none";
        }
        else{
            category = @event.Category!.Name;
        }


        return new(
            @event.Id,
            @event.Title,
            @event.Description,
            @event.StartTime,
            @event.EndTime,
            category,
            @event.User.Username,
            @event.CreatedAt,
            EventParticipants,
            @event.IsChecked
        );
    }
}
