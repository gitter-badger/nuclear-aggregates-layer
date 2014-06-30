using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Qds.Etl.Transform.Docs;
using DoubleGis.Erm.TaskService.DI;
using DoubleGis.Erm.TaskService.Settings;

using FluentAssertions;

using Machine.Specifications;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.TaskService.Tests.Unit
{
    class BootstrapperSpecs
    {
        // TODO Вернуть эти тесты когда elastic работа эластика будет не на этапе Resolve

        [Subject(typeof(Bootstrapper))]
        class When_resolve_type_client_grid_doc : BootstrapperResolveTypeContext<ClientGridDocQdsComponent>
        {
            //[Ignore("Эластик конфигурируется в Bootstraper")]
            Behaves_like<ResolvedAsBehavior<ClientGridDocQdsComponent>> successiful;
        }

        [Subject(typeof(Bootstrapper))]
        class When_resolve_type_territory_doc : BootstrapperResolveTypeContext<TerritoryDocQdsComponent>
        {
            //[Ignore("Эластик конфигурируется в Bootstraper")]
            Behaves_like<ResolvedAsBehavior<TerritoryDocQdsComponent>> successiful;
        }

        [Subject(typeof(Bootstrapper))]
        class When_resolve_type_user_doc : BootstrapperResolveTypeContext<UserDocQdsComponent>
        {
            //[Ignore("Эластик конфигурируется в Bootstraper")]
            Behaves_like<ResolvedAsBehavior<UserDocQdsComponent>> successiful;
        }

        [Subject(typeof(Bootstrapper))]
        class When_resolve_type_indexing_process : BootstrapperResolveTypeContext<IIndexingProcess>
        {
            //[Ignore("Эластик конфигурируется в Bootstraper")]
            Behaves_like<ResolvedAsBehavior<BatchIndexingProcess>> successiful;
        }

        class BootstrapperResolveTypeContext<TResolve> : BootstrapperContext
        {
            Because of = () => Result = Container.Resolve<TResolve>();
        }

        [Behaviors]
        class ResolvedAsBehavior<TResolved>
        {
            protected static object Result;

            It should_be_resolved_as_type = () => Result.Should().NotBeNull().And.BeOfType<TResolved>();
        }

        class BootstrapperContext
        {
            Establish context = () =>
                {
                    ServiceSettings = new TaskServiceAppSettings(BusinessModels.Supported);
                    Container = Bootstrapper.ConfigureUnity(ServiceSettings);
                };

            protected static TaskServiceAppSettings ServiceSettings { get; private set; }
            protected static IUnityContainer Container;
            protected static object Result;
        }
    }
}