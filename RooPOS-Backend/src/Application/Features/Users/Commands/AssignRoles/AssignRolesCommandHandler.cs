using Application.Common.Exceptions;
using Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Users.Commands.AssignRoles;

public class AssignRolesCommandHandler : ICommandHandler<AssignRolesCommand>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AssignRolesCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task Handle(AssignRolesCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId)
            ?? throw new NotFoundException("User not found");

        var currentRoles = await _userManager.GetRolesAsync(user);
        var rolesToAdd = request.Roles.Except(currentRoles);
        var rolesToRemove = currentRoles.Except(request.Roles);

        if (rolesToAdd.Any())
        {
            var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
            if (!addResult.Succeeded)
            {
                throw new Exception($"Failed to add roles: {string.Join(", ", addResult.Errors.Select(e => e.Description))}");
            }
        }
        if (rolesToRemove.Any())
        {
            var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            if (!removeResult.Succeeded)
            {
                throw new Exception($"Failed to remove roles: {string.Join(", ", removeResult.Errors.Select(e => e.Description))}");
            }
        }
    }
}
