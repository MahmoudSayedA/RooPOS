using Application.Common.Abstractions.Data;
using Domain.Entities.Users;

namespace Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand>
{
    private readonly IApplicationDbContext _dbContext;
    public UpdateUserCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Set<ApplicationUser>().FindAsync(new object[] { request.Id }, cancellationToken)
            ?? throw new KeyNotFoundException("User not found.");

        if (!string.IsNullOrEmpty(request.FullName))
            user.FullName = request.FullName;
        if (!string.IsNullOrEmpty(request.UserName))
            user.UserName = request.UserName;
        if (!string.IsNullOrEmpty(request.Email))
            user.Email = request.Email;
        if (!string.IsNullOrEmpty(request.PhoneNumber))
            user.PhoneNumber = request.PhoneNumber;
        if (request.IsActive.HasValue)
            user.IsActive = request.IsActive.Value;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
