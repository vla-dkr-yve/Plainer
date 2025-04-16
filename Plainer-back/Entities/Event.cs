namespace Plainer.Entities;

public class Event
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
    public int CreatedBy { get; set; }
    public User User { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public bool IsChecked { get; set; }

    public ICollection<EventParticipant> EventParticipants { get; set; } = new List<EventParticipant>();
}
