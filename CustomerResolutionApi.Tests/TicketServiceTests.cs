using AutoMapper;
using CustomerResolutionApi.DTOs;
using CustomerResolutionApi.Mappings;
using CustomerResolutionApi.Models;
using CustomerResolutionApi.Models.Enums;
using CustomerResolutionApi.Repositories.Interfaces;
using CustomerResolutionApi.Services;
using Moq;

namespace CustomerResolutionApi.Tests;

public class TicketServiceTests
{
    private readonly Mock<ITicketRepository> _ticketRepoMock;
    private readonly IMapper _mapper;
    private readonly TicketService _ticketService;

    public TicketServiceTests()
    {
        _ticketRepoMock = new Mock<ITicketRepository>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _ticketService = new TicketService(_ticketRepoMock.Object, _mapper);
    }

    [Fact]
    public async Task CreateTicketAsync_ReturnsTicketResponse()
    {
        // Arrange
        var request = new CreateTicketRequest
        {
            Title = "Test Ticket",
            Description = "Test Description",
            CategoryId = 1,
            Priority = "High"
        };

        _ticketRepoMock.Setup(r => r.CreateAsync(It.IsAny<Ticket>()))
            .ReturnsAsync((Ticket t) =>
            {
                t.Id = 1;
                t.Category = new Category { Id = 1, CategoryName = "Software" };
                t.Creator = new User { Id = 1, FirstName = "John", LastName = "Doe" };
                return t;
            });

        // Act
        var result = await _ticketService.CreateTicketAsync(request, 1);

        // Assert
        Assert.Equal("Test Ticket", result.Title);
        Assert.Equal("High", result.Priority);
        Assert.Equal("Open", result.Status);
    }

    [Fact]
    public async Task GetTicketsAsync_AsAdmin_ReturnsAllTickets()
    {
        // Arrange
        var tickets = new List<Ticket>
        {
            CreateTestTicket(1, "Ticket 1", 1),
            CreateTestTicket(2, "Ticket 2", 2)
        };

        _ticketRepoMock.Setup(r => r.GetAllAsync(null)).ReturnsAsync(tickets);

        // Act
        var result = await _ticketService.GetTicketsAsync(1, "Admin");

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetTicketsAsync_AsCustomer_ReturnsOwnTickets()
    {
        // Arrange
        var tickets = new List<Ticket> { CreateTestTicket(1, "My Ticket", 5) };
        _ticketRepoMock.Setup(r => r.GetByCreatorAsync(5, null)).ReturnsAsync(tickets);

        // Act
        var result = await _ticketService.GetTicketsAsync(5, "Customer");

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task GetTicketByIdAsync_AsCustomer_CannotSeeOthersTicket()
    {
        // Arrange
        var ticket = CreateTestTicket(1, "Other's Ticket", 99);
        _ticketRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ticket);

        // Act
        var result = await _ticketService.GetTicketByIdAsync(1, 5, "Customer");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateTicketAsync_AssignsTicketAndChangesStatus()
    {
        // Arrange
        var ticket = CreateTestTicket(1, "Open Ticket", 1);
        ticket.Status = TicketStatus.Open;

        _ticketRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ticket);
        _ticketRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Ticket>()))
            .ReturnsAsync((Ticket t) => t);

        var request = new UpdateTicketRequest { AssignedTo = 10 };

        // Act
        var result = await _ticketService.UpdateTicketAsync(1, request, 1, "Admin");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Assigned", result!.Status);
    }

    [Fact]
    public async Task DeleteTicketAsync_AsAdmin_DeletesAnyTicket()
    {
        // Arrange
        var ticket = CreateTestTicket(1, "Any Ticket", 99);
        _ticketRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ticket);
        _ticketRepoMock.Setup(r => r.DeleteAsync(ticket)).Returns(Task.CompletedTask);

        // Act
        var result = await _ticketService.DeleteTicketAsync(1, 1, "Admin");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteTicketAsync_AsCustomer_CannotDeleteOthersTicket()
    {
        // Arrange
        var ticket = CreateTestTicket(1, "Other's Ticket", 99);
        _ticketRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ticket);

        // Act
        var result = await _ticketService.DeleteTicketAsync(1, 5, "Customer");

        // Assert
        Assert.False(result);
    }

    private static Ticket CreateTestTicket(int id, string title, int createdBy)
    {
        return new Ticket
        {
            Id = id,
            Title = title,
            Description = "Description",
            CategoryId = 1,
            Priority = Priority.Medium,
            Status = TicketStatus.Open,
            CreatedBy = createdBy,
            Category = new Category { Id = 1, CategoryName = "Software" },
            Creator = new User { Id = createdBy, FirstName = "User", LastName = $"{createdBy}" }
        };
    }
}
