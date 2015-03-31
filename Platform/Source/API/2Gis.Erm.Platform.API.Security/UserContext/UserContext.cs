using NuClear.Security.API.UserContext;
using NuClear.Security.API.UserContext.Identity;
using NuClear.Security.API.UserContext.Profile;

namespace DoubleGis.Erm.Platform.API.Security.UserContext
{
    public sealed class UserContext : IUserContext, IUserContextModifyAccessor
    {
        private IUserIdentity _identity;
        private IUserProfile _profile;

        public UserContext(IUserIdentity identity, IUserProfile profile)
        {
            _identity = identity;
            _profile = profile;
        }

        public IUserIdentity Identity
        {
            get { return _identity; }
        }

        IUserProfile IUserContextModifyAccessor.Profile 
        { 
            set { _profile = value; }
        }

        IUserIdentity IUserContextModifyAccessor.Identity 
        { 
            set { _identity = value; }
        }

        public IUserProfile Profile
        {
            get { return _profile; }
        }
    }
}
