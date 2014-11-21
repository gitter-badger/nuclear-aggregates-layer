using DoubleGis.Erm.Platform.DI.WCF;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.WCF.BasicOperations.DI;
using DoubleGis.Erm.WCF.BasicOperations.Settings;

namespace DoubleGis.Erm.WCF.BasicOperations
{
    public sealed class UnityServiceHostFactory : UnityServiceHostFactoryBase<OperationsSettings>
    {
        public UnityServiceHostFactory()
            : base(new OperationsSettings(BusinessModels.Supported), Bootstrapper.ConfigureUnity)
        {
        }
    }
}
