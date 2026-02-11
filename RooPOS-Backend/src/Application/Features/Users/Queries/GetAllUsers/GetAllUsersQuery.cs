using Application.Common.Abstractions.Collections;
using Application.Common.Abstractions.Data;
using Application.Common.Extensions;
using Application.Common.Models;
using Application.Features.Users.Models;
using Application.Features.Users.Services;
using Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Users.Queries.GetAllUsers;
public class GetAllUsersQuery : HasTableView, ICommand<PaginatedList<GetUserModel>>
{
}

public class GetAllUsersQueryHandler(
    IUserService userService) : ICommandHandler<GetAllUsersQuery, PaginatedList<GetUserModel>>
{
    private readonly IUserService _userService = userService;
    public async Task<PaginatedList<GetUserModel>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        List<string> allowedFilters = ["FullName", "Email", "UserName", "IsActive"];
        List<string> allowedSorting = ["FullName", "Email", "UserName"];

        request.ValidateFiltersAndSorting(allowedFilters, allowedSorting);

        var users = await _userService.GetAllUsers(request, cancellationToken);
        return users;
    }
}
