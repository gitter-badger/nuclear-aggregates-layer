using DoubleGis.Erm.API.WCF.MoDi.DI;
using DoubleGis.Erm.API.WCF.MoDi.Settings;
using DoubleGis.Erm.Platform.DI.WCF;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.API.WCF.MoDi
{
    public sealed class UnityServiceHostFactory : UnityServiceHostFactoryBase<MoDiAppSettings>
    {
        public UnityServiceHostFactory()
            : base(new MoDiAppSettings(BusinessModels.Supported), Bootstrapper.ConfigureUnity)
        {
        }
    }
}