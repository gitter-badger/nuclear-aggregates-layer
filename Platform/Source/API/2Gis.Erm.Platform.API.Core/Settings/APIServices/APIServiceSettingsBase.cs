using DoubleGis.Erm.Platform.API.Core.Settings.APIServices.Config;

using NuClear.Settings.API;

namespace DoubleGis.Erm.Platform.API.Core.Settings.APIServices
{
    public abstract class APIServiceSettingsBase : ISettingsAspect, IAPIServiceSettingsInitializer
    {
        public abstract string Name { get; }
        public abstract void Initialize(ErmServiceDescription configSettings);
    }
}