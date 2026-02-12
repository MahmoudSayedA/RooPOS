using Domain.Constants;

namespace Application.Features.Users.Commands.AssignRoles;
public class AssignRolesCommand : ICommand
{
    public string UserId { get; set; } = null!;
    public List<string> Roles { get; set; } = new List<string>();
}

// validator
public class AssignRolesCommandValidator : AbstractValidator<AssignRolesCommand>
{
    public AssignRolesCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Roles).NotEmpty();

        RuleForEach(x => x.Roles).Must(r => typeof(Roles).GetFields().Any(f => f.GetValue(null)?.ToString() == r))
            .WithMessage("One or more roles are invalid.");
    }
}