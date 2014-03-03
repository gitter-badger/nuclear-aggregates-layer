using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Qds.Docs;

using Machine.Specifications;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.AcceptanceTests
{
    class ClientGridDocIndexationFacadeSpecs
    {
        [Subject(typeof(IndexationFacade))]
        class When_client_raletion_changed_to_not_indexed_user : RelatedEntityChangedContext<Client, ClientGridDoc>
        {
            Establish context = () =>
            {
                ExpectedUserId = 444;
                ChangedEntity.OwnerCode = ExpectedUserId;

                AddRelatedDocuments("Id", ChangedEntityId, new ClientGridDoc());
            };

            Because of = () => Target.ExecuteEtlFlow();

            It should_publish_related_document = () => PublishedDocsContainRelatedDocuments();
            It should_update_territory_id_field = () => PublishedDocsShouldContain<ClientGridDoc>(d => d.OwnerCode == ExpectedUserId);

            static long ExpectedUserId;
        }

        [Subject(typeof(IndexationFacade))]
        class When_client_raletion_changed_to_not_indexed_territory : RelatedEntityChangedContext<Client, ClientGridDoc>
        {
            Establish context = () =>
            {
                ExpectedTerritoryId = 444;
                ChangedEntity.TerritoryId = ExpectedTerritoryId;

                AddRelatedDocuments("Id", ChangedEntityId, new ClientGridDoc());
            };

            Because of = () => Target.ExecuteEtlFlow();

            It should_publish_related_document = () => PublishedDocsContainRelatedDocuments();
            It should_update_territory_id_field = () => PublishedDocsShouldContain<ClientGridDoc>(d => d.TerritoryId == ExpectedTerritoryId);

            static long ExpectedTerritoryId;
        }

        [Subject(typeof(IndexationFacade))]
        class When_client_relation_to_territory_changed : RelatedEntityChangedContext<Client, ClientGridDoc>
        {
            Establish context = () =>
            {
                const long newId = 42;
                const long oldId = 111;

                ChangedEntity.TerritoryId = newId;

                ExpectedTerritoryName = "Territory name";
                AddToDocsStorage(new TerritoryDoc { Name = ExpectedTerritoryName }, "Id", newId);

                AddRelatedDocuments("Id", ChangedEntityId, new ClientGridDoc { TerritoryId = oldId });
            };

            Because of = () => Target.ExecuteEtlFlow();

            It should_update_territory_name_from_territory_doc = () => PublishedDocsShouldContain<ClientGridDoc>(doc => doc.TerritoryName == ExpectedTerritoryName);

            static string ExpectedTerritoryName;
        }

        [Subject(typeof(IndexationFacade))]
        class When_client_relation_to_user_changed : RelatedEntityChangedContext<Client, ClientGridDoc>
        {
            Establish context = () =>
            {
                const long newId = 42;
                const long oldId = 111;

                ChangedEntity.OwnerCode = newId;

                ExpectedOwnerName = "Owner name";
                AddToDocsStorage(new UserDoc { Name = ExpectedOwnerName }, "Id", newId);

                AddRelatedDocuments("Id", ChangedEntityId, new ClientGridDoc { OwnerCode = oldId });
            };

            Because of = () => Target.ExecuteEtlFlow();

            It should_update_owner_name_from_user_doc = () => PublishedDocsShouldContain<ClientGridDoc>(doc => doc.OwnerName == ExpectedOwnerName);

            static string ExpectedOwnerName;
        }

        [Subject(typeof(IndexationFacade))]
        class When_territory_entity_changed_and_has_linked_clients : RelatedEntityChangedContext<Territory, ClientGridDoc>
        {
            Establish context = () => AddRelatedDocuments("TerritoryId", ChangedEntityId, new ClientGridDoc(), new ClientGridDoc());

            Because of = () => Target.ExecuteEtlFlow();

            It should_update_linked_documents = () => PublishedDocsContainRelatedDocuments();
        }

        [Subject(typeof(IndexationFacade))]
        class When_user_entity_changed_and_has_linked_clients : RelatedEntityChangedContext<User, ClientGridDoc>
        {
            Establish context = () => AddRelatedDocuments("OwnerCode", ChangedEntityId, new ClientGridDoc(), new ClientGridDoc());

            Because of = () => Target.ExecuteEtlFlow();

            It should_update_linked_documents = () => PublishedDocsContainRelatedDocuments();
        }

        [Subject(typeof(IndexationFacade))]
        class When_client_entity_changed : EntityChangedContext<Client, ClientGridDoc>
        {
            Because of = () => Target.ExecuteEtlFlow();

            It should_publish_document_with_expected_id = () => ContainDocumentOfSpecifiedTypeWithExpectedId();
        }
    }
}