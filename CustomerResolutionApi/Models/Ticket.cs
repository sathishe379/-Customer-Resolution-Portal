using CustomerResolutionApi.Models.Enums;

namespace CustomerResolutionApi.Models;

public class Ticket : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public Priority Priority { get; set; } = Priority.Medium;
    public TicketStatus Status { get; set; } = TicketStatus.Open;
    public int CreatedBy { get; set; }
    public int? AssignedTo { get; set; }
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

    public Category Category { get; set; } = null!;
    public User Creator { get; set; } = null!;
    public User? Assignee { get; set; }
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
