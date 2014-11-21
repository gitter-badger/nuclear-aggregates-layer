using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.DI.Config
{
    interface IContainerFactory
    {
        IUnityContainer CreateAndConfigure();
    }
}