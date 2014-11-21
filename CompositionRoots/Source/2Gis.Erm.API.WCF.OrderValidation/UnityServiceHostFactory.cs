using DoubleGis.Erm.API.WCF.OrderValidation.DI;
using DoubleGis.Erm.API.WCF.OrderValidation.Settings;
using DoubleGis.Erm.Platform.DI.WCF;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.API.WCF.OrderValidation
{
    public sealed class UnityServiceHostFactory : UnityServiceHostFactoryBase<OrderValidationAppSettings>
    {
        public UnityServiceHostFactory()
            : base(new OrderValidationAppSettings(BusinessModels.Supported), Bootstrapper.ConfigureUnity)
        {
        }
    }
}
