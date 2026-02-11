using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Users.Models;
public class GetUserModel
{
    public Guid Id { get; set; }
    public string? FullName { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public string? UserName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<string>? Roles { get; set; } = new();
}
