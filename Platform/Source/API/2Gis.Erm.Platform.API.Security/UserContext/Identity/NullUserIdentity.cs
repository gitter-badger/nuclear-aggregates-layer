using NuClear.Security.API.UserContext.Identity;

namespace DoubleGis.Erm.Platform.API.Security.UserContext.Identity
{
    public sealed class NullUserIdentity : IUserIdentity
    {
        private readonly UserInfo _userInfo;
        public NullUserIdentity()
        {
            _userInfo = UserInfo.Empty;
        }

        public bool SkipEntityAccessCheck { get; set; }

        public string DisplayName
        {
            get { return _userInfo.DisplayName; }
        }

        public long Code
        {
            get { return _userInfo.Code; }
        }

        public string Account
        {
            get { return _userInfo.Account; }
        }

        public UserInfo UserData
        {
            get { return _userInfo; }
        }
    }
}
