using CustomerResolutionApi.DTOs;

namespace CustomerResolutionApi.Services.Interfaces;

public interface ITicketService
{
    Task<IEnumerable<TicketResponse>> GetTicketsAsync(int userId, string role, TicketFilterRequest? filter = null);
    Task<TicketResponse?> GetTicketByIdAsync(int id, int userId, string role);
    Task<TicketResponse> CreateTicketAsync(CreateTicketRequest request, int userId);
    Task<TicketResponse?> UpdateTicketAsync(int id, UpdateTicketRequest request, int userId, string role);
    Task<bool> DeleteTicketAsync(int id, int userId, string role);
    Task<DashboardResponse> GetDashboardAsync(int userId, string role);
}
