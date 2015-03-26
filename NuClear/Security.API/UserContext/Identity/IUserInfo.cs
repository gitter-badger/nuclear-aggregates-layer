namespace DoubleGis.Erm.Platform.API.Security.UserContext.Identity
{
    public interface IUserInfo
    {
        string DisplayName { get; }
        long Code { get; }
        string Account { get; }
    }
}