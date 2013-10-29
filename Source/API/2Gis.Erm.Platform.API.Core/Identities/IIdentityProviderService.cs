
namespace DoubleGis.Erm.Platform.API.Core.Identities
{
    /// <summary>
    /// Сервис генерации уникальных ID
    /// </summary>
    public interface IIdentityProviderService
    {
        long[] GetIdentities(int count);
    }
}
