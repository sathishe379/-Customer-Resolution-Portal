namespace CustomerResolutionApi.Models;

public class Comment : BaseEntity
{
    public int TicketId { get; set; }
    public string Content { get; set; } = string.Empty;
    public int CreatedBy { get; set; }

    public Ticket Ticket { get; set; } = null!;
    public User Author { get; set; } = null!;
}
