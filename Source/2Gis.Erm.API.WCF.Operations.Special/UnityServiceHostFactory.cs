using DoubleGis.Erm.API.WCF.Operations.Special.DI;
using DoubleGis.Erm.API.WCF.Operations.Special.Settings;
using DoubleGis.Erm.Platform.DI.WCF;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.API.WCF.Operations.Special
{
    public sealed class UnityServiceHostFactory : UnityServiceHostFactoryBase<FinancialOperationsAppSettings>
    {
        public UnityServiceHostFactory()
            : base(new FinancialOperationsAppSettings(BusinessModels.Supported), Bootstrapper.ConfigureUnity)
        {
        }
    }
}
