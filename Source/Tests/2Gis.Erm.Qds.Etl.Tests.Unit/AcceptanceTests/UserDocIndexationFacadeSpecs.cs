using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Qds.Docs;

using Machine.Specifications;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.AcceptanceTests
{
    class UserDocIndexationFacadeSpecs
    {
        [Subject(typeof(IndexationFacade))]
        class When_user_entity_changed : EntityChangedContext<User, UserDoc>
        {
            Because of = () => Target.ExecuteEtlFlow();

            It user_doc_should_be_published = () => ContainDocumentOfSpecifiedTypeWithExpectedId();
        }
    }
}