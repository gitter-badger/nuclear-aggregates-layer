using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.Platform.Common.Identities
{
    public interface IIdentityProviderSettings : ISettings
    {
        IdentityServiceUniqueIdSource UniqueIdSource { get; }
    }
}
