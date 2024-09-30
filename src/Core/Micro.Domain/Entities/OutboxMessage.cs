namespace Micro.Domain.Entities;

public class OutboxMessage : Entity<int>
{
    public string EventType { get; set; }
    
    public string EventPayload { get; set; }
    
    public DateTime EventDate { get; set; } = DateTime.UtcNow;
    
    public bool IsSent { get; set; }
    
    public DateTime? SentDate { get; set; }
}