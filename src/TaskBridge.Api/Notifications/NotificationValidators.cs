using FluentValidation;

namespace TaskBridge.Api.Notifications;

public sealed class CreateNotificationRequestValidator : AbstractValidator<CreateNotificationRequest>
{
    public CreateNotificationRequestValidator()
    {
        RuleFor(request => request.RecipientUserId).NotEmpty();
        RuleFor(request => request.Type).IsInEnum();
        RuleFor(request => request.Title).NotEmpty().MaximumLength(200);
        RuleFor(request => request.Body).NotEmpty().MaximumLength(4000);
    }
}