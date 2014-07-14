using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Qds.Docs;
using DoubleGis.Erm.Qds.Etl.Extract.EF;

using FluentAssertions;

using Machine.Specifications;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.AcceptanceTests
{
    class IndexationFacadeSpecs
    {
        // FIXME {f.zaharov, 23.04.2014}: Тест, что обновилось состояние последней обработанной операции

        [Subject(typeof(IndexationFacade))]
        class When_continously_execute_etl_flow_without_changes : IndexationFacadeContext
        {
            Establish context = () =>
            {
                var entity = new Client { Id = 42, };

                Target.LogChangesForEntity(entity);
                Target.ExecuteEtlFlow();
                DocsStorage.ClearPublishedDocs();
            };

            Because of = () => Target.ExecuteEtlFlow();

            It should_not_index_anything = () => DocsStorage.NewPublishedDocs.Should().BeEmpty();
        }

        [Subject(typeof(IndexationFacade))]
        class When_unsupported_entity_changed : IndexationFacadeContext
        {
            Establish context = () =>
            {
                var unsupportedEntity = new ReleaseInfo { Id = 42 };

                Target.LogChangesForEntity(unsupportedEntity);
            };

            Because of = () => Target.ExecuteEtlFlow();

            It should_not_be_indexed = () => DocsStorage.NewPublishedDocs.Should().OnlyContain(doc => doc is RecordIdState);
        }
    }
}
