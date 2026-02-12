using Application.Features.Users.Commands.AssignRoles;
using Application.Features.Users.Commands.UpdateUser;
using Application.Features.Users.Queries.GetAllUsers;
using Application.Identity.Services;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;

namespace Web.Controllers;


[Route("api/users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IUser _user;

    public UsersController(ISender sender, IUser user)
    {
        _sender = sender;
        _user = user;
    }

    [HttpGet("roles")]
    public ActionResult<List<string>> GetRoles()
    {
        var userId = _user.Id;
        var roles = typeof(Roles).GetFields().Select(f => f.GetValue(null)?.ToString()).ToList();
        return Ok(roles);
    }

    [HttpPost("get-all")]
    [Authorize]
    public async Task<IActionResult> GetAll([FromBody] GetAllUsersQuery command, CancellationToken cancellationToken)
    {
        var users = await _sender.Send(command, cancellationToken);
        return Ok(users);
    }
    [HttpPut("update")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update([FromBody] UpdateUserCommand command, CancellationToken cancellationToken)
    {
        await _sender.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPost("assign-roles")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignRoles([FromBody] AssignRolesCommand command, CancellationToken cancellationToken)
    {
        await _sender.Send(command, cancellationToken);
        return NoContent();
    }

}
