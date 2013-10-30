using DoubleGis.Erm.BL.WCF.Operations.Settings;
using DoubleGis.Erm.Platform.DI.WCF;
using DoubleGis.Erm.WCF.BasicOperations.DI;
using DoubleGis.Erm.WCF.BasicOperations.Settings;

namespace DoubleGis.Erm.WCF.BasicOperations
{
    public sealed class UnityServiceHostFactory : UnityServiceHostFactoryBase<IBasicOperationsSettings, BasicOperationsSettings>
    {
        public UnityServiceHostFactory()
            : base(Bootstrapper.ConfigureUnity)
        {
        }
    }
}
