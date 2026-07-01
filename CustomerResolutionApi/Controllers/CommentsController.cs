using CustomerResolutionApi.DTOs;
using CustomerResolutionApi.Helpers;
using CustomerResolutionApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerResolutionApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpGet("{ticketId}")]
    public async Task<ActionResult<IEnumerable<CommentResponse>>> GetComments(int ticketId)
    {
        var result = await _commentService.GetByTicketIdAsync(ticketId);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<CommentResponse>> CreateComment([FromBody] CreateCommentRequest request)
    {
        var userId = User.GetUserId();
        var result = await _commentService.CreateAsync(request, userId);
        return CreatedAtAction(nameof(GetComments), new { ticketId = result.TicketId }, result);
    }
}
