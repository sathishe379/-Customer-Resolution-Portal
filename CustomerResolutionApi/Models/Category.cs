namespace CustomerResolutionApi.Models;

public class Category : BaseEntity
{
    public string CategoryName { get; set; } = string.Empty;
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
