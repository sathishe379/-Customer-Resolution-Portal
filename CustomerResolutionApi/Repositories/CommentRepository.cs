using CustomerResolutionApi.Data;
using CustomerResolutionApi.Models;
using CustomerResolutionApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CustomerResolutionApi.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly AppDbContext _context;

    public CommentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Comment>> GetByTicketIdAsync(int ticketId)
    {
        return await _context.Comments
            .Include(c => c.Author)
            .Where(c => c.TicketId == ticketId)
            .OrderByDescending(c => c.CreatedDate)
            .ToListAsync();
    }

    public async Task<Comment> CreateAsync(Comment comment)
    {
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return await _context.Comments
            .Include(c => c.Author)
            .FirstAsync(c => c.Id == comment.Id);
    }
}
