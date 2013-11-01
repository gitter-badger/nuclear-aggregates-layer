using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;

namespace DoubleGis.Erm.Platform.WCF.Metadata.Settings
{
    public interface IMetadataServiceAppSettings : IAppSettings, IIdentityProviderSettings, IMsCrmSettingsHost, IAPIServiceSettingsHost
    {
    }
}