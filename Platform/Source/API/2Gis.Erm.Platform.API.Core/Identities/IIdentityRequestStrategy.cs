namespace DoubleGis.Erm.Platform.API.Core.Identities
{
    public interface IIdentityRequestStrategy
    {
        long[] Request(int count);
    }
}