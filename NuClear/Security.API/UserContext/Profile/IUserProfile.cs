namespace DoubleGis.Erm.Platform.API.Security.UserContext.Profile
{
    public interface IUserProfile
    {
        LocaleInfo UserLocaleInfo { get; }
        long UserCode { get; }
    }
}