using DoubleGis.Erm.Qds.IndexService.DI;
using DoubleGis.Erm.Qds.IndexService.Settings;

using FluentAssertions;

using Machine.Specifications;

using Microsoft.Practices.Unity;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Qds.IndexService.Tests.Unit
{
    class BootstrapperSpecs
    {
        [Subject(typeof(Bootstrapper))]
        class When_resolve_indexing_process : BootstrapperContext
        {
            Because of = () => Container = Bootstrapper.ConfigureUnity(ServiceSettings);

            It should_be_batch_indexing_process = () => Container.Resolve<IIndexingProcess>().Should().BeOfType<BatchIndexingProcess>();
        }

        [Subject(typeof(Bootstrapper))]
        class When_configure_unity : BootstrapperContext
        {
            Because of = () => Container = Bootstrapper.ConfigureUnity(ServiceSettings);

            It should_create_unity_container = () => Container.Should().NotBeNull();
        }

        class BootstrapperContext
        {
            Establish context = () =>
                {
                    ServiceSettings = new IndexServiceAppSettings();
                };

            protected static IIndexServiceAppSettings ServiceSettings { get; private set; }
            protected static IUnityContainer Container;
        }
    }
}