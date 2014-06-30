using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Qds.Docs;

using Machine.Specifications;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.AcceptanceTests
{
    [Subject(typeof(IndexationFacade))]
    class OrderGridDocIndexationFacadeSpecs
    {
        [Subject(typeof(IndexationFacade))]
        class When_order_entity_changed : EntityChangedContext<Order, OrderGridDoc>
        {
            Because of = () => Target.ExecuteEtlFlow();

            It should_publish_document_with_expected_id = () => ContainDocumentOfSpecifiedTypeWithExpectedId();
        }
    }
}