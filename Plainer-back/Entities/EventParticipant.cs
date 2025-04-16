namespace Plainer.Entities;

public class EventParticipant
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int EventId { get; set; }
    public Event Event { get; set; } = null!;

    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
}
