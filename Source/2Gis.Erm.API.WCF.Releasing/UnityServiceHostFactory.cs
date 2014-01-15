using DoubleGis.Erm.API.WCF.Releasing.DI;
using DoubleGis.Erm.API.WCF.Releasing.Settings;
using DoubleGis.Erm.BLCore.WCF.Releasing.Settings;
using DoubleGis.Erm.Platform.DI.WCF;

namespace DoubleGis.Erm.API.WCF.Releasing
{
    public sealed class UnityServiceHostFactory : UnityServiceHostFactoryBase<IReleasingSettings, ReleasingSettings>
    {
        public UnityServiceHostFactory() 
            : base(Bootstrapper.ConfigureUnity)
        {
        }
    }
}
