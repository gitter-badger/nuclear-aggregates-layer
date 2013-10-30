using DoubleGis.Erm.API.WCF.OrderValidation.DI;
using DoubleGis.Erm.API.WCF.OrderValidation.Settings;
using DoubleGis.Erm.BL.WCF.OrderValidation.Settings;
using DoubleGis.Erm.Platform.DI.WCF;

namespace DoubleGis.Erm.API.WCF.OrderValidation
{
    public sealed class UnityServiceHostFactory : UnityServiceHostFactoryBase<IOrderValidationAppSettings, OrderValidationAppSettings>
    {
        public UnityServiceHostFactory()
            : base(Bootstrapper.ConfigureUnity)
        {
        }
    }
}
