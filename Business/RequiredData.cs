using Data.Entities;

namespace Business;

public static class RequiredData
{
    public static readonly IEnumerable<string> Roles = new[] {"Manager", "Admin"};

    public static readonly User Admin = new()
    {
        UserName = "admin",
        Email = "admin@gamestore.com",
        FirstName = "Ivan",
        LastName = "Ivanov"
    };

    public const string AdminPassword = "adminpass";
}