using Nuclear.Settings.API;

namespace DoubleGis.Erm.Platform.Common.Identities
{
    public interface IIdentityProviderSettings : ISettings
    {
        int IdentityServiceUniqueId { get; }
    }
}
