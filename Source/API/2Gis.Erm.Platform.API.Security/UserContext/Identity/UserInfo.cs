using System;

namespace DoubleGis.Erm.Platform.API.Security.UserContext.Identity
{
    [Serializable]
    public class UserInfo : IUserInfo
    {
        private readonly long _code;
        private readonly string _account;
        private readonly string _displayName;

        private static readonly UserInfo EmptyInfo = new UserInfo(-1, null, null);

        public UserInfo(long code, string account, string displayName)
        {
            _code = code;
            _account = account;
            _displayName = displayName;
        }

        // serialization constructor
        private UserInfo()
        {
        }

        public string DisplayName
        {
            get { return _displayName; }
        }

        public string Account
        {
            get { return _account; }
        }

        public long Code
        {
            get { return _code; }
        }

        public static UserInfo Empty
        {
            get { return EmptyInfo; }
        }
    }
}