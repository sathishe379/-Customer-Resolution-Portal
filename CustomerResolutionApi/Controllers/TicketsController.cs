using CustomerResolutionApi.DTOs;
using CustomerResolutionApi.Helpers;
using CustomerResolutionApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerResolutionApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;

    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TicketResponse>>> GetTickets([FromQuery] TicketFilterRequest? filter)
    {
        var userId = User.GetUserId();
        var role = User.GetUserRole();
        var result = await _ticketService.GetTicketsAsync(userId, role, filter);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TicketResponse>> GetTicket(int id)
    {
        var userId = User.GetUserId();
        var role = User.GetUserRole();
        var result = await _ticketService.GetTicketByIdAsync(id, userId, role);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<TicketResponse>> CreateTicket([FromBody] CreateTicketRequest request)
    {
        var userId = User.GetUserId();
        var result = await _ticketService.CreateTicketAsync(request, userId);
        return CreatedAtAction(nameof(GetTicket), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TicketResponse>> UpdateTicket(int id, [FromBody] UpdateTicketRequest request)
    {
        var userId = User.GetUserId();
        var role = User.GetUserRole();
        var result = await _ticketService.UpdateTicketAsync(id, request, userId, role);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTicket(int id)
    {
        var userId = User.GetUserId();
        var role = User.GetUserRole();
        var deleted = await _ticketService.DeleteTicketAsync(id, userId, role);
        if (!deleted) return NotFound();
        return NoContent();
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult<DashboardResponse>> GetDashboard()
    {
        var userId = User.GetUserId();
        var role = User.GetUserRole();
        var result = await _ticketService.GetDashboardAsync(userId, role);
        return Ok(result);
    }
}
