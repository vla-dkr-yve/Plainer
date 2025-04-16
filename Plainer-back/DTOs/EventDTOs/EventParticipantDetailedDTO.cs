namespace Plainer.DTOs.EventDTOs;

public record class EventParticipantDetailedDTO(
    int UserId,
    string UserName,
    int RoleId,
    string RoleName
);
