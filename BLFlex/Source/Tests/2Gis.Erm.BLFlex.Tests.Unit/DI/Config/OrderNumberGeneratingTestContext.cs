using System;

using Machine.Specifications;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.DI.Config
{
    abstract class OrderNumberGeneratingTestContext<TService, TContainerFactory>
        where TContainerFactory : IContainerFactory, new()
    {
        protected const string Source1CSyncCode = "666";
        protected const string Dest1CSyncCode = "999";
        protected const int ReservedNumber = 1;

        protected static TService Service;
        protected static string GeneratedNumber;
        protected static Exception Exception;

        Establish context = () => Service = new TContainerFactory().CreateAndConfigure().Resolve<TService>();
    }
}