using Application.Common.Validations;

namespace Application.Features.Users.Commands.UpdateUser;
public class UpdateUserCommand : ICommand
{
    public Guid Id { get; set; }
    public string? FullName { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public bool? IsActive { get; set; }

}

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("User Id is required.");
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("Invalid email format.");

        RuleFor(x => x.PhoneNumber)
            .Cascade(CascadeMode.Stop)  // stop on first failure
            .NotEmpty().When(x => x.PhoneNumber != null)  // or .NotNull() if you prefer
            .Matches(@"^\+[1-9]\d{1,14}$")                 // must start with +, no leading zero, 1–15 digits total
            .MinimumLength(4)                             // shortest realistic: +120 (3 digits + prefix)
            .MaximumLength(15)                            // E.164 max
            .Must(ValidationHelper.BeValidCountryCode)                     // optional – see below
            .WithMessage("Please enter a valid international phone number (e.g. +201234567890)");
    }
}