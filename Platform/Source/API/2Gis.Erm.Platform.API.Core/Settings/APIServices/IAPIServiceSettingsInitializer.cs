using DoubleGis.Erm.Platform.API.Core.Settings.APIServices.Config;

namespace DoubleGis.Erm.Platform.API.Core.Settings.APIServices
{
    public interface IAPIServiceSettingsInitializer
    {
        void Initialize(ErmServiceDescription configSettings);
    }
}