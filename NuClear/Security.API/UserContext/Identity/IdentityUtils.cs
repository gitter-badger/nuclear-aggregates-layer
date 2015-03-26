using System.Security.Principal;

namespace DoubleGis.Erm.Platform.API.Security.UserContext.Identity
{
    public static class IdentityUtils
    {
         public static string GetAccount(IIdentity identity)
         {
             return identity.Name.Contains("\\") ? identity.Name.Split('\\')[1] : identity.Name;
         }

        public static string GetAccount(string domainAccount)
        {
            var s = domainAccount.Split('\\');
            return s[s.Length - 1];
        }
    }
}