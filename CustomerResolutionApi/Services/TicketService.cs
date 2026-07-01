using AutoMapper;
using CustomerResolutionApi.DTOs;
using CustomerResolutionApi.Models;
using CustomerResolutionApi.Models.Enums;
using CustomerResolutionApi.Repositories.Interfaces;
using CustomerResolutionApi.Services.Interfaces;

namespace CustomerResolutionApi.Services;

public class TicketService : ITicketService
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IMapper _mapper;

    public TicketService(ITicketRepository ticketRepository, IMapper mapper)
    {
        _ticketRepository = ticketRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TicketResponse>> GetTicketsAsync(int userId, string role, TicketFilterRequest? filter = null)
    {
        IEnumerable<Ticket> tickets = role switch
        {
            "Admin" => await _ticketRepository.GetAllAsync(filter),
            "SupportEngineer" => await _ticketRepository.GetByAssigneeAsync(userId, filter),
            _ => await _ticketRepository.GetByCreatorAsync(userId, filter)
        };

        return _mapper.Map<IEnumerable<TicketResponse>>(tickets);
    }

    public async Task<TicketResponse?> GetTicketByIdAsync(int id, int userId, string role)
    {
        var ticket = await _ticketRepository.GetByIdAsync(id);
        if (ticket == null) return null;

        // Customers can only see their own tickets
        if (role == "Customer" && ticket.CreatedBy != userId) return null;
        // Support engineers can see assigned tickets
        if (role == "SupportEngineer" && ticket.AssignedTo != userId) return null;

        return _mapper.Map<TicketResponse>(ticket);
    }

    public async Task<TicketResponse> CreateTicketAsync(CreateTicketRequest request, int userId)
    {
        if (!Enum.TryParse<Priority>(request.Priority, out var priority))
            throw new ArgumentException("Invalid priority value.");

        var ticket = new Ticket
        {
            Title = request.Title,
            Description = request.Description,
            CategoryId = request.CategoryId,
            Priority = priority,
            Status = TicketStatus.Open,
            CreatedBy = userId
        };

        var created = await _ticketRepository.CreateAsync(ticket);
        return _mapper.Map<TicketResponse>(created);
    }

    public async Task<TicketResponse?> UpdateTicketAsync(int id, UpdateTicketRequest request, int userId, string role)
    {
        var ticket = await _ticketRepository.GetByIdAsync(id);
        if (ticket == null) return null;

        // Authorization checks
        if (role == "Customer" && ticket.CreatedBy != userId) return null;
        if (role == "SupportEngineer" && ticket.AssignedTo != userId) return null;

        if (request.Title != null) ticket.Title = request.Title;
        if (request.Description != null) ticket.Description = request.Description;
        if (request.CategoryId.HasValue) ticket.CategoryId = request.CategoryId.Value;

        if (request.Priority != null && Enum.TryParse<Priority>(request.Priority, out var priority))
            ticket.Priority = priority;

        if (request.Status != null && Enum.TryParse<TicketStatus>(request.Status, out var status))
            ticket.Status = status;

        if (request.AssignedTo.HasValue && (role == "Admin" || role == "SupportEngineer"))
        {
            ticket.AssignedTo = request.AssignedTo.Value;
            if (ticket.Status == TicketStatus.Open)
                ticket.Status = TicketStatus.Assigned;
        }

        var updated = await _ticketRepository.UpdateAsync(ticket);
        return _mapper.Map<TicketResponse>(updated);
    }

    public async Task<bool> DeleteTicketAsync(int id, int userId, string role)
    {
        var ticket = await _ticketRepository.GetByIdAsync(id);
        if (ticket == null) return false;

        if (role != "Admin" && ticket.CreatedBy != userId) return false;

        await _ticketRepository.DeleteAsync(ticket);
        return true;
    }

    public async Task<DashboardResponse> GetDashboardAsync(int userId, string role)
    {
        return await _ticketRepository.GetDashboardStatsAsync(userId, role);
    }
}
