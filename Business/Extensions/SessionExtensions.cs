using Business.Interfaces;

namespace Business.Extensions;

public static class SessionExtensions
{
   public static bool IsManager(this ISession session)
   {
      return session.IsAuthorized && session.UserRoles.Contains("Manager");
   }
}