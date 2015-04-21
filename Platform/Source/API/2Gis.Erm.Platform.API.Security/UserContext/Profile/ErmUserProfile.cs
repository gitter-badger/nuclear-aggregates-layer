using System;

using NuClear.Security.API.UserContext.Profile;

namespace DoubleGis.Erm.Platform.API.Security.UserContext.Profile
{
    [Serializable]
    public sealed class ErmUserProfile : IUserProfile
    {
        public ErmUserProfile(long userCode, LocaleInfo localeInfo)
        {
            UserCode = userCode;
            UserLocaleInfo = localeInfo;
        }

        public long UserCode { get; private set; }
        public LocaleInfo UserLocaleInfo { get; private set; }
    }
}