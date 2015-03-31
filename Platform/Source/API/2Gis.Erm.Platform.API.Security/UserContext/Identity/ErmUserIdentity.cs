using NuClear.Security.API.UserContext.Identity;

namespace DoubleGis.Erm.Platform.API.Security.UserContext.Identity
{
    public sealed class ErmUserIdentity : IUserIdentity
    {
        private readonly UserInfo _userInfo;

        public ErmUserIdentity(UserInfo userInfo)
        {
            _userInfo = userInfo;
        }

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

        public bool SkipEntityAccessCheck { get; set; }

        public UserInfo UserData
        {
            get { return _userInfo; }
        }
    } 
}