using AutoMapper;
using CustomerResolutionApi.DTOs;
using CustomerResolutionApi.Models;

namespace CustomerResolutionApi.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, UserResponse>()
            .ForMember(d => d.Role, opt => opt.MapFrom(s => s.Role.ToString()));

        // Ticket mappings
        CreateMap<Ticket, TicketResponse>()
            .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category.CategoryName))
            .ForMember(d => d.CreatorName, opt => opt.MapFrom(s => $"{s.Creator.FirstName} {s.Creator.LastName}"))
            .ForMember(d => d.AssigneeName, opt => opt.MapFrom(s => s.Assignee != null ? $"{s.Assignee.FirstName} {s.Assignee.LastName}" : null))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.Priority, opt => opt.MapFrom(s => s.Priority.ToString()));

        // Category mappings
        CreateMap<Category, CategoryResponse>();

        // Comment mappings
        CreateMap<Comment, CommentResponse>()
            .ForMember(d => d.AuthorName, opt => opt.MapFrom(s => $"{s.Author.FirstName} {s.Author.LastName}"));
    }
}
