using DoubleGis.Erm.Platform.API.Core.Settings.APIServices.Config;
using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.Platform.API.Core.Settings.APIServices
{
    public abstract class APIServiceSettingsBase : ISettingsAspect, IAPIServiceSettingsInitializer
    {
        public abstract string Name { get; }
        public abstract void Initialize(ErmServiceDescription configSettings);
    }
}