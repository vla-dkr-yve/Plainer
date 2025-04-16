namespace Plainer.DTOs.EventDTOs;

public record class UpdateEventDTO(
    string Title, 
    string Description, 
    DateTime StartTime,
    DateTime? EndTime, 
    int CategoryId,
    List<EventParticipantDTO> EventParticipants,
    bool IsChecked
);