namespace Plainer.DTOs.EventDTOs;

public record class EventDTO(
    int Id, 
    string Title, 
    string Description, 
    DateTime StartTime, 
    DateTime? EndTime, 
    int? CategoryId,
    int CreatedBy,
    DateTime CreatedAt,
    int NumberOfParticipants,
    bool IsChecked
);