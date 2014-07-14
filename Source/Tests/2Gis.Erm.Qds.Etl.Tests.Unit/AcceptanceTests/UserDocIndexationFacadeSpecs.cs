using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Qds.Docs;

using Machine.Specifications;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.AcceptanceTests
{
    class UserDocIndexationFacadeSpecs
    {
        [Ignore("Cut for ERM-4267")]
        [Subject(typeof(IndexationFacade))]
        class When_user_entity_changed : EntityChangedContext<User, UserDoc>
        {
            Establish context = () =>
            {
                ExpectedDoc = Target.ExpectDocument<User, UserDoc>(new UserDoc());
            };

            Because of = () => Target.ExecuteEtlFlow();

            It user_doc_should_be_published = () => DocsStorage.NewPublishedDocs.Contains(ExpectedDoc);

            static UserDoc ExpectedDoc;
        }
    }
}