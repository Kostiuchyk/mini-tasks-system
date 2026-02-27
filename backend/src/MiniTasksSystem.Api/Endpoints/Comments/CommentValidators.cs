using FluentValidation;

namespace MiniTasksSystem.Api.Endpoints.Comments;

public sealed class CreateCommentRequestValidator : AbstractValidator<CreateCommentRequest>
{
    public CreateCommentRequestValidator()
    {
        RuleFor(x => x.Text).NotEmpty().MaximumLength(2000);
    }
}
