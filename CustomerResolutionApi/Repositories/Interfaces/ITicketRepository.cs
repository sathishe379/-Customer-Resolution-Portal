using CustomerResolutionApi.DTOs;
using CustomerResolutionApi.Models;

namespace CustomerResolutionApi.Repositories.Interfaces;

public interface ITicketRepository
{
    Task<Ticket?> GetByIdAsync(int id);
    Task<IEnumerable<Ticket>> GetAllAsync(TicketFilterRequest? filter = null);
    Task<IEnumerable<Ticket>> GetByCreatorAsync(int userId, TicketFilterRequest? filter = null);
    Task<IEnumerable<Ticket>> GetByAssigneeAsync(int userId, TicketFilterRequest? filter = null);
    Task<Ticket> CreateAsync(Ticket ticket);
    Task<Ticket> UpdateAsync(Ticket ticket);
    Task DeleteAsync(Ticket ticket);
    Task<DashboardResponse> GetDashboardStatsAsync(int? userId = null, string? role = null);
}
