using DoubleGis.Erm.Platform.Common.Identities;
using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.Platform.API.Core.Identities
{
    public class IdentityProviderSettingsAspect : IIdentityProviderSettings, ISettingsAspect
    {
        private readonly EnumSetting<IdentityServiceUniqueIdSource> _uniqueIdSource = ConfigFileSetting.Enum.Required<IdentityServiceUniqueIdSource>("IdentityServiceUniqueIdSource");

        public IdentityServiceUniqueIdSource UniqueIdSource
        {
            get { return _uniqueIdSource.Value; }
        }
    }
}