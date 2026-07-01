namespace CustomerResolutionApi.DTOs;

public class CreateCommentRequest
{
    public int TicketId { get; set; }
    public string Content { get; set; } = string.Empty;
}

public class CommentResponse
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public string Content { get; set; } = string.Empty;
    public int CreatedBy { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}
