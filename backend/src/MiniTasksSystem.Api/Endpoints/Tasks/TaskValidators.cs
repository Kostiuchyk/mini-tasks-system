using FluentValidation;

namespace MiniTasksSystem.Api.Endpoints.Tasks;

public sealed class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>
{
    private static readonly string[] ValidStatuses = ["New", "Active", "Done"];
    private static readonly string[] ValidPriorities = ["Low", "Medium", "High"];

    public CreateTaskRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.Status).NotEmpty().Must(s => ValidStatuses.Contains(s))
            .WithMessage("Status must be one of: New, Active, Done.");
        RuleFor(x => x.Priority).NotEmpty().Must(p => ValidPriorities.Contains(p))
            .WithMessage("Priority must be one of: Low, Medium, High.");
        RuleFor(x => x.ProjectId).NotEmpty();
    }
}

public sealed class UpdateTaskRequestValidator : AbstractValidator<UpdateTaskRequest>
{
    private static readonly string[] ValidStatuses = ["New", "Active", "Done"];
    private static readonly string[] ValidPriorities = ["Low", "Medium", "High"];

    public UpdateTaskRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.Status).NotEmpty().Must(s => ValidStatuses.Contains(s))
            .WithMessage("Status must be one of: New, Active, Done.");
        RuleFor(x => x.Priority).NotEmpty().Must(p => ValidPriorities.Contains(p))
            .WithMessage("Priority must be one of: Low, Medium, High.");
    }
}
