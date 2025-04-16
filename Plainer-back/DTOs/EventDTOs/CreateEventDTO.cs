namespace Plainer.DTOs.EventDTOs;

public record class CreateEventDTO(
    string Title, 
    string Description, 
    DateTime StartTime, 
    DateTime? EndTime, 
    int? CategoryId
);