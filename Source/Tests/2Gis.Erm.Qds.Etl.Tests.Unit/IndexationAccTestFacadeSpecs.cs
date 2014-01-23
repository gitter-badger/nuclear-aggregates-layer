using System;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Qds.Etl.Extract.EF;
using DoubleGis.Erm.Qds.Etl.Transform.Docs;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit
{
    // TODO Многовато копипасты, надо поработать над этим.
    class IndexationAccTestFacadeSpecs
    {
        [Subject(typeof(IndexationAccTestFacade))]
        class When_unsupported_entity_changed : IndexationAccTestFacadeContext
        {
            Establish context = () =>
            {
                var unsupportedEntity = new ReleaseInfo { Id = 42 };

                Target.MockChangesForEntity(unsupportedEntity);
            };

            Because of = () => Target.ExecuteEtlFlow();

            It should_not_be_indexed = () => DocsStorage.PublishedDocs.Should().OnlyContain(doc => doc is RecordIdState);
        }

        [Subject(typeof(IndexationAccTestFacade))]
        class When_client_field_changed : IndexationAccTestFacadeContext
        {
            Establish context = () =>
                {
                    ExpectedMainAddress = "new main address";
                    var entity = new Client { Id = 42, MainAddress = ExpectedMainAddress };

                    PutDocToDataBase(new ClientGridDoc(), "Id", entity.Id);

                    Target.MockChangesForEntity(entity);
                };

            Because of = () => Target.ExecuteEtlFlow();

            It should_update_client_grid_doc_field = () => IndexedDocsShouldContain<ClientGridDoc>(doc => doc.MainAddress == ExpectedMainAddress);

            static string ExpectedMainAddress;
        }

        [Subject(typeof(IndexationAccTestFacade))]
        class When_client_enum_field_changed : IndexationAccTestFacadeContext
        {
            Establish context = () =>
                {
                    ExpectedInformationSource = "Localized Information Source";

                    const int infSource = (int)InformationSource.Media;
                    EnumLocalizer.Setup(l => l.GetLocalizedString((InformationSource)infSource)).Returns(ExpectedInformationSource);

                    var entity = new Client { Id = 42, InformationSource = infSource };

                    PutDocToDataBase(new ClientGridDoc(), "Id", entity.Id);
                    Target.MockChangesForEntity(entity);
                };

            Because of = () => Target.ExecuteEtlFlow();

            It should_localize_field_value_as_string =
                () => IndexedDocsShouldContain<ClientGridDoc>(doc => doc.InformationSource == ExpectedInformationSource);

            static string ExpectedInformationSource;
        }

        [Subject(typeof(IndexationAccTestFacade))]
        class When_user_changed : IndexationAccTestFacadeContext
        {
            Establish context = () =>
                {
                    ExpectedName = "Юзер Петрович";
                    var entity = new User { Id = 42, DisplayName = ExpectedName };

                    PutDocToDataBase(new UserDoc(), "Id", entity.Id);
                    PutDocToDataBase(new ClientGridDoc(), "OwnerCode", entity.Id);
                    PutDocToDataBase(new ClientGridDoc(), "OwnerCode", entity.Id);

                    Target.MockChangesForEntity(entity);
                };

            Because of = () => Target.ExecuteEtlFlow();

            It should_update_main_and_linked_docs = () => DocsStorage.PublishedDocs.Should().HaveCount(4);
            It should_update_user_doc_field = () => IndexedDocsShouldContain<UserDoc>(doc => doc.Name == ExpectedName);
            It should_update_linked_client_grid_docs = () => IndexedDocsShouldContain<ClientGridDoc>(doc => doc.OwnerName == ExpectedName);

            static string ExpectedName;
        }

        [Subject(typeof(IndexationAccTestFacade))]
        class When_territory_changed : IndexationAccTestFacadeContext
        {
            Establish context = () =>
            {
                ExpectedName = "Area 51";
                var entity = new Territory { Id = 42, Name = ExpectedName };

                PutDocToDataBase(new TerritoryDoc(), "Id", entity.Id);
                PutDocToDataBase(new ClientGridDoc(), "TerritoryId", entity.Id);
                PutDocToDataBase(new ClientGridDoc(), "TerritoryId", entity.Id);

                Target.MockChangesForEntity(entity);
            };

            Because of = () => Target.ExecuteEtlFlow();

            It should_update_main_and_linked_docs = () => DocsStorage.PublishedDocs.Should().HaveCount(4);
            It should_update_user_doc_field = () => IndexedDocsShouldContain<TerritoryDoc>(doc => doc.Name == ExpectedName);
            It should_update_linked_client_grid_docs = () => IndexedDocsShouldContain<ClientGridDoc>(doc => doc.TerritoryName == ExpectedName);

            static string ExpectedName;
        }

        [Subject(typeof(IndexationAccTestFacade))]
        class When_new_client_added : IndexationAccTestFacadeContext
        {
            Establish context = () =>
                {
                    ExpectedOwnerName = "Owner name";
                    ExpectedTerritoryName = "Territor name";

                    const long ownerCode = 42;
                    const long territoryId = 444;
                    ExpectedId = 111;

                    PutDocToDataBase(new UserDoc { Id = ownerCode, Name = ExpectedOwnerName }, "Id", ownerCode);
                    PutDocToDataBase(new TerritoryDoc { Id = territoryId, Name = ExpectedTerritoryName }, "Id", territoryId);

                    var client = new Client { Id = ExpectedId, OwnerCode = ownerCode, TerritoryId = territoryId };

                    Target.MockChangesForEntity(client);
                };

            Because of = () => Target.ExecuteEtlFlow();

            It should_add_new_client_grid_doc = () => IndexedDocsShouldContain<ClientGridDoc>(doc => doc.Id == ExpectedId);
            It should_load_owner_name = () => IndexedDocsShouldContain<ClientGridDoc>(doc => doc.OwnerName == ExpectedOwnerName);
            It should_load_territory = () => IndexedDocsShouldContain<ClientGridDoc>(doc => doc.TerritoryName == ExpectedTerritoryName);

            static string ExpectedOwnerName;
            static string ExpectedTerritoryName;
            static long ExpectedId;
        }

        [Subject(typeof(IndexationAccTestFacade))]
        class When_new_client_added_with_not_indexed_territory_or_owner : IndexationAccTestFacadeContext
        {
            Establish context = () =>
                {
                    ExpectedOwnerCode = 555;
                    ExpectedTerritoryId = 444;
                    ExpectedId = 111;

                    var client = new Client { Id = ExpectedId, TerritoryId = ExpectedTerritoryId, OwnerCode = ExpectedOwnerCode };
                    //PutDocToDataBase(new UserDoc { Id = ownerCode }, "Id", ownerCode);

                    Target.MockChangesForEntity(client);
                };

            Because of = () => Target.ExecuteEtlFlow();

            // TODO Надо сделать Behaves_like для проиндексированного документа и обновления последней обработанной записи
            It should_index_only_one_doc_and_update_record_id = () => DocsStorage.PublishedDocs.Should().HaveCount(2);
            It should_add_new_client_grid_doc = () => IndexedDocsShouldContain<ClientGridDoc>(doc => doc.Id == ExpectedId);
            It should_set_territory_id = () => IndexedDocsShouldContain<ClientGridDoc>(doc => doc.TerritoryId == ExpectedTerritoryId);
            It should_set_owner_code = () => IndexedDocsShouldContain<ClientGridDoc>(doc => doc.OwnerCode == ExpectedOwnerCode);
            It should_leave_territory_name_null = () => IndexedDocsShouldContain<ClientGridDoc>(doc => doc.TerritoryName == null);
            It should_leave_owner_name_null = () => IndexedDocsShouldContain<ClientGridDoc>(doc => doc.OwnerName == null);

            static long ExpectedTerritoryId;
            static long ExpectedOwnerCode;
            static long ExpectedId;
        }

        [Subject(typeof(IndexationAccTestFacade))]
        class When_client_territory_and_owner_changed : IndexationAccTestFacadeContext
        {
            Establish context = () =>
            {
                ExpectedOwnerName = "new Owner name";
                ExpectedTerritoryName = "new Territor name";

                const long newOwnerCode = 42;
                const long newTerritoryId = 444;
                ExpectedId = 111;

                PutDocToDataBase(new ClientGridDoc { Id = ExpectedId }, "Id", newOwnerCode);
                PutDocToDataBase(new UserDoc { Id = newOwnerCode, Name = ExpectedOwnerName }, "Id", newOwnerCode);
                PutDocToDataBase(new TerritoryDoc { Id = newTerritoryId, Name = ExpectedTerritoryName }, "Id", newTerritoryId);

                var client = new Client { Id = ExpectedId, OwnerCode = newOwnerCode, TerritoryId = newTerritoryId };

                Target.MockChangesForEntity(client);
            };

            Because of = () => Target.ExecuteEtlFlow();

            It should_update_client_grid_doc = () => IndexedDocsShouldContain<ClientGridDoc>(doc => doc.Id == ExpectedId);
            It should_load_owner_name = () => IndexedDocsShouldContain<ClientGridDoc>(doc => doc.OwnerName == ExpectedOwnerName);
            It should_load_territory_name = () => IndexedDocsShouldContain<ClientGridDoc>(doc => doc.TerritoryName == ExpectedTerritoryName);

            static string ExpectedOwnerName;
            static string ExpectedTerritoryName;
            static long ExpectedId;
        }

        [Subject(typeof(IndexationAccTestFacade))]
        class When_new_user_added : IndexationAccTestFacadeContext
        {
            Establish context = () =>
                {
                    ExpectedId = 42;
                    Target.MockChangesForEntity(new User { Id = ExpectedId });
                };

            Because of = () => Target.ExecuteEtlFlow();

            It should_add_new_user_doc = () => IndexedDocsShouldContain<UserDoc>(doc => doc.Id == ExpectedId);

            static long ExpectedId;
        }

        [Subject(typeof(IndexationAccTestFacade))]
        class When_new_territory_added : IndexationAccTestFacadeContext
        {
            Establish context = () =>
                {
                    ExpectedId = 42;
                    ExpectedStateId = Target.MockChangesForEntity(new Territory { Id = ExpectedId });
                };

            Because of = () => Target.ExecuteEtlFlow();

            It should_add_new_territory_doc = () => IndexedDocsShouldContain<TerritoryDoc>(doc => doc.Id == ExpectedId);
            It should_update_state = () => IndexedDocsShouldContain<RecordIdState>(state => state.RecordId == ExpectedStateId);

            static long ExpectedId;
            static long ExpectedStateId;
        }

        class IndexationAccTestFacadeContext
        {
            Establish context = () =>
                {
                    EnumLocalizer = new Mock<IEnumLocalizer>();
                    DocsStorage = new MockDocsStorage();
                    var queryDsl = new MockDocsStorage.QueryDsl();

                    LastRecordStateId = 888;
                    TrackerState = new DocsStorageChangesTrackerState(DocsStorage, queryDsl);
                    DocsStorage.Add(new RecordIdState(0, LastRecordStateId), DocsStorageChangesTrackerState.IdFieldName, (long)0);

                    // Todo разбить на три отдеьных acceptance теста для UserDoc, TerritoryDoc и ClientGridDoc
                    var clientQds = new ClientGridDocQdsComponent(DocsStorage, EnumLocalizer.Object, queryDsl);
                    var userQds = new UserDocQdsComponent(DocsStorage, queryDsl);
                    var territoryQds = new TerritoryDocQdsComponent(DocsStorage, queryDsl);

                    Target = new IndexationAccTestFacade(DocsStorage, TrackerState, clientQds, userQds, territoryQds);
                };

            protected static long LastRecordStateId { get; private set; }
            protected static Mock<IEnumLocalizer> EnumLocalizer { get; private set; }
            protected static IndexationAccTestFacade Target { get; private set; }
            protected static MockDocsStorage DocsStorage { get; private set; }
            protected static DocsStorageChangesTrackerState TrackerState { get; private set; }

            protected static void PutDocToDataBase<TDoc>(TDoc doc, string fieldName, object value) where TDoc : IDoc
            {
                DocsStorage.Add(doc, fieldName, value);
            }

            protected static void IndexedDocsShouldContain<TDoc>(Func<TDoc, bool> checkDoc)
            {
                DocsStorage.PublishedDocs.Should()
                    .NotBeEmpty()
                    .And
                    .Contain(doc => doc is TDoc && checkDoc((TDoc)doc));
            }
        }
    }
}
