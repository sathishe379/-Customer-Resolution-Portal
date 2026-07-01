using CustomerResolutionApi.DTOs;
using FluentValidation;

namespace CustomerResolutionApi.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(100);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6).MaximumLength(100);
    }
}

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class CreateTicketRequestValidator : AbstractValidator<CreateTicketRequest>
{
    private static readonly string[] ValidPriorities = { "Low", "Medium", "High", "Critical" };

    public CreateTicketRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.CategoryId).GreaterThan(0);
        RuleFor(x => x.Priority).Must(p => ValidPriorities.Contains(p))
            .WithMessage("Priority must be one of: Low, Medium, High, Critical");
    }
}

public class UpdateTicketRequestValidator : AbstractValidator<UpdateTicketRequest>
{
    private static readonly string[] ValidPriorities = { "Low", "Medium", "High", "Critical" };
    private static readonly string[] ValidStatuses = { "Open", "Assigned", "InProgress", "Resolved", "Closed" };

    public UpdateTicketRequestValidator()
    {
        RuleFor(x => x.Title).MaximumLength(200).When(x => x.Title != null);
        RuleFor(x => x.Description).MaximumLength(2000).When(x => x.Description != null);
        RuleFor(x => x.CategoryId).GreaterThan(0).When(x => x.CategoryId.HasValue);
        RuleFor(x => x.Priority).Must(p => ValidPriorities.Contains(p))
            .When(x => x.Priority != null)
            .WithMessage("Priority must be one of: Low, Medium, High, Critical");
        RuleFor(x => x.Status).Must(s => ValidStatuses.Contains(s))
            .When(x => x.Status != null)
            .WithMessage("Status must be one of: Open, Assigned, InProgress, Resolved, Closed");
    }
}

public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(x => x.CategoryName).NotEmpty().MaximumLength(100);
    }
}

public class CreateCommentRequestValidator : AbstractValidator<CreateCommentRequest>
{
    public CreateCommentRequestValidator()
    {
        RuleFor(x => x.TicketId).GreaterThan(0);
        RuleFor(x => x.Content).NotEmpty().MaximumLength(2000);
    }
}
