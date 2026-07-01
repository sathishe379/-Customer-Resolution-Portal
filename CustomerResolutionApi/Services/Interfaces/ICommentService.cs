using CustomerResolutionApi.DTOs;

namespace CustomerResolutionApi.Services.Interfaces;

public interface ICommentService
{
    Task<IEnumerable<CommentResponse>> GetByTicketIdAsync(int ticketId);
    Task<CommentResponse> CreateAsync(CreateCommentRequest request, int userId);
}
