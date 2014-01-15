using DoubleGis.Erm.API.WCF.Operations.Special.DI;
using DoubleGis.Erm.API.WCF.Operations.Special.Settings;
using DoubleGis.Erm.BLCore.WCF.Operations.Special.FinancialOperations.Settings;
using DoubleGis.Erm.Platform.DI.WCF;

namespace DoubleGis.Erm.API.WCF.Operations.Special
{
    public sealed class UnityServiceHostFactory : UnityServiceHostFactoryBase<IFinancialOperationsAppSettings, FinancialOperationsAppSettings>
    {
        public UnityServiceHostFactory()
            : base(Bootstrapper.ConfigureUnity)
        {
        }
    }
}
