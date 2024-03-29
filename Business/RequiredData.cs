﻿using Data.Entities;

namespace Business;

public static class RequiredData
{
    public static IEnumerable<string> Roles => new[] {"Manager", "Admin"};

    public static User Admin => new()
    {
        UserName = "admin",
        Email = "admin@gamestore.com",
        FirstName = "Ivan",
        LastName = "Ivanov"
    };

    public const string AdminPassword = "adminpass";
}