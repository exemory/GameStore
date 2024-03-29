﻿using Microsoft.AspNetCore.Identity;

namespace Data.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string? Avatar { get; set; }

        public IEnumerable<Comment> Comments { get; set; } = default!;
        public IEnumerable<UserRole> UserRoles { get; set; } = default!;
    }
}