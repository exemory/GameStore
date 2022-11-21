using Microsoft.AspNetCore.Identity;

namespace Data.Entities;

public class Role : IdentityRole<Guid>
{
    public Role()
    {
    }

    public Role(string roleName) : base(roleName)
    {
    }

    public IEnumerable<UserRole> UserRoles { get; set; } = default!;
}