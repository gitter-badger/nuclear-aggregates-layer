namespace DoubleGis.Erm.Platform.API.Security.UserContext.Profile
{
    public sealed class ErmUserProfile : IUserProfile
    {
        public long UserCode { get; private set; }
        public LocaleInfo UserLocaleInfo { get; private set; }

        public ErmUserProfile(long userCode, LocaleInfo localeInfo)
        {
            UserCode = userCode;
            UserLocaleInfo = localeInfo;
        }
    }
}