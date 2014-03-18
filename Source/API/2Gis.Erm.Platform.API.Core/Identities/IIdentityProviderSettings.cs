using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.Platform.API.Core.Identities
{
    public interface IIdentityProviderSettings : ISettings
    {
        int IdentityServiceUniqueId { get; }
    }
}
