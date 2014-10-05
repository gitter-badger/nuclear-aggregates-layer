namespace DoubleGis.Erm.Platform.Common.Identities
{
    /// <summary>
    /// Сервис генерации уникальных ID
    /// </summary>
    public interface IIdentityProviderService
    {
        long[] GetIdentities(int count);
    }
}
