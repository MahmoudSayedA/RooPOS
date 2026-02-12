using Application.Common.Models;
using Application.Features.Users.Models;
using Application.Features.Users.Queries.GetAllUsers;
using Application.Features.Users.Services;
using Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Users;
internal class UserService : IUserService
{
    private readonly ApplicationDbContext _dbContext;

    public UserService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaginatedList<GetUserModel>> GetAllUsers(GetAllUsersQuery request, CancellationToken cancellationToken)
    {

        var whereClauses = new List<string>();
        var parameters = new List<SqlParameter>();

        // Filters
        if (request.Filters?.Any() == true)
        {
            foreach (var filter in request.Filters)
            {
                var value = filter.Value?.Trim();
                if (string.IsNullOrEmpty(value)) continue;

                var paramName = $"@p{parameters.Count}";

                switch (filter.Key)
                {
                    case "FullName":
                        whereClauses.Add($"FullName LIKE {paramName}");
                        parameters.Add(new SqlParameter(paramName, $"%{value}%"));
                        break;

                    case "Email":
                        whereClauses.Add($"Email LIKE {paramName}");
                        parameters.Add(new SqlParameter(paramName, $"%{value}%"));
                        break;

                    case "UserName":
                        whereClauses.Add($"UserName LIKE {paramName}");
                        parameters.Add(new SqlParameter(paramName, $"%{value}%"));
                        break;

                    case "IsActive":
                        if (bool.TryParse(value, out bool isActive))
                        {
                            whereClauses.Add($"IsActive = {paramName}");
                            parameters.Add(new SqlParameter(paramName, isActive));
                        }
                        break;
                }
            }
        }

        string whereSql = whereClauses.Any()
            ? "WHERE " + string.Join(" AND ", whereClauses)
            : "";

        // Sorting
        string orderBy = "ORDER BY UserName ASC";

        if (!string.IsNullOrEmpty(request.SortBy))
        {
            string direction = string.Equals(request.SortDirection, "desc", StringComparison.OrdinalIgnoreCase)
                ? "DESC"
                : "ASC";

            string column = request.SortBy switch
            {
                "FullName" => "FullName",
                "Email" => "Email",
                "UserName" => "UserName",
                _ => "UserName"
            };

            orderBy = $"ORDER BY {column} {direction}";
        }

        // Paging
        int offset = (request.PageNumber - 1) * request.PageSize;
        int pageSize = request.PageSize;

        parameters.Add(new SqlParameter("@offset", offset));
        parameters.Add(new SqlParameter("@pageSize", pageSize));

        string sql = $@"
        SELECT 
            u.Id,
            u.FullName,
            u.Email,
            u.UserName,
            u.IsActive,
            ISNULL(STRING_AGG(r.Name, ', '), '') AS Roles
        FROM AspNetUsers u
        LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
        LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
        {whereSql}
        GROUP BY u.Id, u.FullName, u.Email, u.UserName, u.IsActive
        {orderBy}
        OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

        string countSql = $@"
        SELECT COUNT(DISTINCT u.Id)
        FROM AspNetUsers u
        {whereSql}";

        var countParams = parameters.Take(parameters.Count - 2).ToArray();

        var users = new List<GetUserModel>();

        // 1. جلب البيانات الرئيسية
        await using var dataConnection = _dbContext.Database.GetDbConnection();
        await dataConnection.OpenAsync(cancellationToken);

        await using var dataCommand = dataConnection.CreateCommand();
        dataCommand.CommandText = sql;
        foreach (var p in parameters) dataCommand.Parameters.Add(p);

        await using var reader = await dataCommand.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync())
        {
            var rolesString = reader.IsDBNull(5) ? "" : reader.GetString(5);
            users.Add(new GetUserModel
            {
                Id = reader.GetGuid(0),
                FullName = reader.IsDBNull(1) ? null : reader.GetString(1),
                Email = reader.IsDBNull(2) ? null : reader.GetString(2),
                UserName = reader.IsDBNull(3) ? null : reader.GetString(3),
                IsActive = reader.GetBoolean(4),
                Roles = [.. rolesString.Split([", "], StringSplitOptions.RemoveEmptyEntries)]
            });
        }
        await dataConnection.CloseAsync();

        // 2. جلب العدد الكلي (connection منفصل)
        int totalCount = 0;
        await using var countConnection = _dbContext.Database.GetDbConnection();
        await countConnection.OpenAsync(cancellationToken);

        await using var countCommand = countConnection.CreateCommand();
        countCommand.CommandText = countSql;
        foreach (var p in countParams) countCommand.Parameters.Add(p);

        var scalarResult = await countCommand.ExecuteScalarAsync(cancellationToken);
        totalCount = scalarResult is int cnt ? cnt : (int)(scalarResult ?? 0);

        await countConnection.CloseAsync();

        return new PaginatedList<GetUserModel>(
            users,
            totalCount,
            request.PageNumber,
            request.PageSize);

    }
}
