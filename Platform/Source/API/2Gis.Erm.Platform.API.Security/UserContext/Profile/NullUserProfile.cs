using NuClear.Security.API.UserContext.Profile;

namespace DoubleGis.Erm.Platform.API.Security.UserContext.Profile
{
    public sealed class NullUserProfile : IUserProfile
    {
        public LocaleInfo UserLocaleInfo
        {
            get { return LocaleInfo.Default; }
        }

        public long UserCode
        {
            get { return 0; }
        }
    }
}
