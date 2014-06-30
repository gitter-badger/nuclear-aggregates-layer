using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Qds.Docs;

using Machine.Specifications;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.AcceptanceTests
{
    class ClientGridDocIndexationFacadeSpecs
    {
        [Ignore("Cut for ERM-4267")]
        [Subject(typeof(IndexationFacade))]
        class When_legal_person_client_id_changed : RelatedEntityChangedContext<LegalPerson, ClientGridDoc>
        {
            Establish context = () =>
            {
                ChangedEntity.ClientId = NewClientId;

                AddRelatedDocuments("Id", NewClientId, new ClientGridDoc { Id = "42" });
                AddRelatedDocuments("LegalPersons.Id", ChangedEntityId, new ClientGridDoc
                    {
                        Id = PreviousClientId,
                        LegalPersons = new[]
                            {
                                new LegalPersonDoc
                                    {
                                        Id = ChangedEntityId.ToString()
                                    }
                            }
                    });
            };

            Because of = () => Target.ExecuteEtlFlow();

            It should_publish_updated_doc_with_removed_acoount_from_legal_person = () =>
                PublishedDocsShouldContain<ClientGridDoc>(d => d.Id == PreviousClientId && d.LegalPersons.Length == 0);

            It should_publish_updated_doc_with_added_acoount_to_legal_person = () =>
                PublishedDocsShouldContain<ClientGridDoc>(d => d.Id == NewClientId.ToString() && d.LegalPersons.Any(lp => lp.Id == ChangedEntityId.ToString()));

            const long NewClientId = 42;
            const string PreviousClientId = "33";
        }

        [Ignore("Cut for ERM-4267")]
        [Subject(typeof(IndexationFacade))]
        class When_account_legal_person_id_changed : RelatedEntityChangedContext<Account, ClientGridDoc>
        {
            Establish context = () =>
                {
                    ChangedEntity.LegalPersonId = NewLegalPersonId;

                    AddRelatedDocuments("LegalPersons.Id", NewLegalPersonId, new ClientGridDoc
                    {
                        LegalPersons = new[] { CreateLegalPersonDoc(NewLegalPersonId.ToString(), new AccountDoc[0]) }
                    });

                    AddRelatedDocuments("LegalPersons.Accounts.Id", ChangedEntityId, new ClientGridDoc
                    {
                        LegalPersons = new[] { CreateLegalPersonDoc(PreviousLegalPersonId, new AccountDoc { Id = ChangedEntityId.ToString(), }) }
                    });
                };

            Because of = () => Target.ExecuteEtlFlow();

            It should_publish_updated_doc_with_removed_acoount_from_legal_person = () =>
                PublishedDocsShouldContain<ClientGridDoc>(d => d.LegalPersons.Any(lp => lp.Id == PreviousLegalPersonId && lp.Accounts.Length == 0));

            It should_publish_updated_doc_with_added_acoount_to_legal_person = () =>
                PublishedDocsShouldContain<ClientGridDoc>(d => d.LegalPersons.Any(lp => lp.Id == NewLegalPersonId.ToString() && lp.Accounts.Any(a => a.Id == ChangedEntityId.ToString())));

            protected static LegalPersonDoc CreateLegalPersonDoc(string id, params AccountDoc[] accountDocs)
            {
                return new LegalPersonDoc
                {
                    Id = id,
                    Accounts = accountDocs
                };
            }

            const long NewLegalPersonId = 42;
            const string PreviousLegalPersonId = "33";
        }

        [Ignore("Cut for ERM-4267")]
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
            It should_update_territory_id_field = () => PublishedDocsShouldContain<ClientGridDoc>(d => d.OwnerCode == ExpectedUserId.ToString());

            static long ExpectedUserId;
        }

        [Ignore("Cut for ERM-4267")]
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
            It should_update_territory_id_field = () => PublishedDocsShouldContain<ClientGridDoc>(d => d.TerritoryId == ExpectedTerritoryId.ToString());

            static long ExpectedTerritoryId;
        }

        [Ignore("Cut for ERM-4267")]
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

                AddRelatedDocuments("Id", ChangedEntityId, new ClientGridDoc { TerritoryId = oldId.ToString() });
            };

            Because of = () => Target.ExecuteEtlFlow();

            It should_update_territory_name_from_territory_doc = () => PublishedDocsShouldContain<ClientGridDoc>(doc => doc.TerritoryName == ExpectedTerritoryName);

            static string ExpectedTerritoryName;
        }

        [Ignore("Cut for ERM-4267")]
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

                AddRelatedDocuments("Id", ChangedEntityId, new ClientGridDoc { OwnerCode = oldId.ToString() });
            };

            Because of = () => Target.ExecuteEtlFlow();

            It should_update_owner_name_from_user_doc = () => PublishedDocsShouldContain<ClientGridDoc>(doc => doc.OwnerName == ExpectedOwnerName);

            static string ExpectedOwnerName;
        }

        [Ignore("Cut for ERM-4267")]
        [Subject(typeof(IndexationFacade))]
        class When_territory_entity_changed_and_has_linked_clients : RelatedEntityChangedContext<Territory, ClientGridDoc>
        {
            Establish context = () => AddRelatedDocuments("TerritoryId", ChangedEntityId, new ClientGridDoc(), new ClientGridDoc());

            Because of = () => Target.ExecuteEtlFlow();

            It should_update_linked_documents = () => PublishedDocsContainRelatedDocuments();
        }

        [Ignore("Cut for ERM-4267")]
        [Subject(typeof(IndexationFacade))]
        class When_user_entity_changed_and_has_linked_clients : RelatedEntityChangedContext<User, ClientGridDoc>
        {
            Establish context = () => AddRelatedDocuments("OwnerCode", ChangedEntityId, new ClientGridDoc(), new ClientGridDoc());

            Because of = () => Target.ExecuteEtlFlow();

            It should_update_linked_documents = () => PublishedDocsContainRelatedDocuments();
        }

        [Ignore("Cut for ERM-4267")]
        [Subject(typeof(IndexationFacade))]
        class When_client_entity_changed : EntityChangedContext<Client, ClientGridDoc>
        {
            Because of = () => Target.ExecuteEtlFlow();

            It should_publish_document_with_expected_id = () => ContainDocumentOfSpecifiedTypeWithExpectedId();
        }
    }
}