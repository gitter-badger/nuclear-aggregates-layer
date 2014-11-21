using System.Security.Principal;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.Security
{
    public sealed class WcfWindowsPrincipal: IPrincipal
    {
        private readonly IIdentity _identity;

        public WcfWindowsPrincipal(IIdentity identity)
        {
            _identity = identity;
        }

        public IIdentity Identity
        {
            get { return _identity; }
        }

        public bool IsInRole(string role)
        {
            return false;
        }
    }
}
