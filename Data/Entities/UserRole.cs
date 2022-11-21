using Microsoft.AspNetCore.Identity;

namespace Data.Entities;

public class UserRole : IdentityUserRole<Guid>
{
    public User User { get; set; } = default!;
    public Role Role { get; set; } = default!;
}