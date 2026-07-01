using AutoMapper;
using CustomerResolutionApi.DTOs;
using CustomerResolutionApi.Models;
using CustomerResolutionApi.Repositories.Interfaces;
using CustomerResolutionApi.Services.Interfaces;

namespace CustomerResolutionApi.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly IMapper _mapper;

    public CommentService(ICommentRepository commentRepository, IMapper mapper)
    {
        _commentRepository = commentRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CommentResponse>> GetByTicketIdAsync(int ticketId)
    {
        var comments = await _commentRepository.GetByTicketIdAsync(ticketId);
        return _mapper.Map<IEnumerable<CommentResponse>>(comments);
    }

    public async Task<CommentResponse> CreateAsync(CreateCommentRequest request, int userId)
    {
        var comment = new Comment
        {
            TicketId = request.TicketId,
            Content = request.Content,
            CreatedBy = userId
        };

        var created = await _commentRepository.CreateAsync(comment);
        return _mapper.Map<CommentResponse>(created);
    }
}
