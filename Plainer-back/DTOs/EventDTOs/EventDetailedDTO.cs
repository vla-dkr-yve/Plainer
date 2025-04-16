namespace Plainer.DTOs.EventDTOs;

public record class EventDetailedDTO(
    int Id, 
    string Title, 
    string Description, 
    DateTime StartTime, 
    DateTime? EndTime, 
    string? CategoryName,
    string CreatedBy,
    DateTime CreatedAt,
    List<EventParticipantDetailedDTO> EventParticipants,
    bool IsChecked
);