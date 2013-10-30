using DoubleGis.Erm.API.WCF.MoDi.DI;
using DoubleGis.Erm.API.WCF.MoDi.Settings;
using DoubleGis.Erm.Platform.DI.WCF;

namespace DoubleGis.Erm.API.WCF.MoDi
{
    public sealed class UnityServiceHostFactory : UnityServiceHostFactoryBase<IMoDiAppSettings, MoDiAppSettings>
    {
        public UnityServiceHostFactory()
            : base(Bootstrapper.ConfigureUnity)
        {
        }
    }
}