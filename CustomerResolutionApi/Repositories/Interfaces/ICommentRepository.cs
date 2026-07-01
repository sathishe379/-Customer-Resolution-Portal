using CustomerResolutionApi.Models;

namespace CustomerResolutionApi.Repositories.Interfaces;

public interface ICommentRepository
{
    Task<IEnumerable<Comment>> GetByTicketIdAsync(int ticketId);
    Task<Comment> CreateAsync(Comment comment);
}
