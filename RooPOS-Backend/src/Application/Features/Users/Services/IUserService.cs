using Application.Common.Models;
using Application.Features.Users.Models;
using Application.Features.Users.Queries.GetAllUsers;

namespace Application.Features.Users.Services;
public interface IUserService
{
    Task<PaginatedList<GetUserModel>> GetAllUsers(GetAllUsersQuery request, CancellationToken cancellationToken);
}
