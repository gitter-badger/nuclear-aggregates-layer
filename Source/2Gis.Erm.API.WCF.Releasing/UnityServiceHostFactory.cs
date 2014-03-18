using DoubleGis.Erm.API.WCF.Releasing.DI;
using DoubleGis.Erm.API.WCF.Releasing.Settings;
using DoubleGis.Erm.Platform.DI.WCF;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.API.WCF.Releasing
{
    public sealed class UnityServiceHostFactory : UnityServiceHostFactoryBase<ReleasingSettings>
    {
        public UnityServiceHostFactory()
            : base(new ReleasingSettings(BusinessModels.Supported), Bootstrapper.ConfigureUnity)
        {
        }
    }
}
