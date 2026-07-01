using CustomerResolutionApi.Data;
using CustomerResolutionApi.DTOs;
using CustomerResolutionApi.Models;
using CustomerResolutionApi.Models.Enums;
using CustomerResolutionApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CustomerResolutionApi.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly AppDbContext _context;

    public TicketRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Ticket?> GetByIdAsync(int id)
    {
        return await _context.Tickets
            .Include(t => t.Category)
            .Include(t => t.Creator)
            .Include(t => t.Assignee)
            .Include(t => t.Comments)
                .ThenInclude(c => c.Author)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Ticket>> GetAllAsync(TicketFilterRequest? filter = null)
    {
        var query = _context.Tickets
            .Include(t => t.Category)
            .Include(t => t.Creator)
            .Include(t => t.Assignee)
            .AsQueryable();

        query = ApplyFilters(query, filter);

        return await query.OrderByDescending(t => t.CreatedDate).ToListAsync();
    }

    public async Task<IEnumerable<Ticket>> GetByCreatorAsync(int userId, TicketFilterRequest? filter = null)
    {
        var query = _context.Tickets
            .Include(t => t.Category)
            .Include(t => t.Creator)
            .Include(t => t.Assignee)
            .Where(t => t.CreatedBy == userId);

        query = ApplyFilters(query, filter);

        return await query.OrderByDescending(t => t.CreatedDate).ToListAsync();
    }

    public async Task<IEnumerable<Ticket>> GetByAssigneeAsync(int userId, TicketFilterRequest? filter = null)
    {
        var query = _context.Tickets
            .Include(t => t.Category)
            .Include(t => t.Creator)
            .Include(t => t.Assignee)
            .Where(t => t.AssignedTo == userId);

        query = ApplyFilters(query, filter);

        return await query.OrderByDescending(t => t.CreatedDate).ToListAsync();
    }

    public async Task<Ticket> CreateAsync(Ticket ticket)
    {
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(ticket.Id) ?? ticket;
    }

    public async Task<Ticket> UpdateAsync(Ticket ticket)
    {
        ticket.UpdatedDate = DateTime.UtcNow;
        _context.Tickets.Update(ticket);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(ticket.Id) ?? ticket;
    }

    public async Task DeleteAsync(Ticket ticket)
    {
        _context.Tickets.Remove(ticket);
        await _context.SaveChangesAsync();
    }

    public async Task<DashboardResponse> GetDashboardStatsAsync(int? userId = null, string? role = null)
    {
        var query = _context.Tickets.AsQueryable();

        if (role == "Customer" && userId.HasValue)
            query = query.Where(t => t.CreatedBy == userId.Value);
        else if (role == "SupportEngineer" && userId.HasValue)
            query = query.Where(t => t.AssignedTo == userId.Value);

        var today = DateTime.UtcNow.Date;
        var tickets = await query.ToListAsync();

        return new DashboardResponse
        {
            TotalTickets = tickets.Count,
            OpenTickets = tickets.Count(t => t.Status == TicketStatus.Open),
            ResolvedTickets = tickets.Count(t => t.Status == TicketStatus.Resolved),
            PendingTickets = tickets.Count(t => t.Status == TicketStatus.Assigned || t.Status == TicketStatus.InProgress),
            TodaysTickets = tickets.Count(t => t.CreatedDate.Date == today)
        };
    }

    private static IQueryable<Ticket> ApplyFilters(IQueryable<Ticket> query, TicketFilterRequest? filter)
    {
        if (filter == null) return query;

        if (!string.IsNullOrEmpty(filter.Title))
            query = query.Where(t => t.Title.Contains(filter.Title));

        if (!string.IsNullOrEmpty(filter.Priority) && Enum.TryParse<Priority>(filter.Priority, out var priority))
            query = query.Where(t => t.Priority == priority);

        if (!string.IsNullOrEmpty(filter.Status) && Enum.TryParse<TicketStatus>(filter.Status, out var status))
            query = query.Where(t => t.Status == status);

        if (filter.CategoryId.HasValue)
            query = query.Where(t => t.CategoryId == filter.CategoryId.Value);

        if (filter.CreatedFrom.HasValue)
            query = query.Where(t => t.CreatedDate >= filter.CreatedFrom.Value);

        if (filter.CreatedTo.HasValue)
            query = query.Where(t => t.CreatedDate <= filter.CreatedTo.Value);

        return query;
    }
}
