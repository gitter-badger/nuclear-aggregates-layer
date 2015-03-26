namespace DoubleGis.Erm.Platform.API.Security.UserContext
{
    public interface IUserContextProvider
    {
        IUserContext Current { get; }
    }
}
