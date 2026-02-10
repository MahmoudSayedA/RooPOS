using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.Users;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? FullName { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow;

}
