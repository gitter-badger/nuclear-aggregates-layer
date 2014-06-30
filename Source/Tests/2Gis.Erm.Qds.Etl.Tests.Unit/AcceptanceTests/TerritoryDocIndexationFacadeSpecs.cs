using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Qds.Docs;

using Machine.Specifications;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.AcceptanceTests
{
    class TerritoryDocIndexationFacadeSpecs
    {
        [Ignore("Cut for ERM-4267")]
        [Subject(typeof(IndexationFacade))]
        class When_territory_entity_changed : EntityChangedContext<Territory, TerritoryDoc>
        {
            Because of = () => Target.ExecuteEtlFlow();

            It territory_doc_should_be_published = () => ContainDocumentOfSpecifiedTypeWithExpectedId();
        }
    }
}