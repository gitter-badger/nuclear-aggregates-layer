using System;

using DoubleGis.Erm.Qds.Etl.Extract.EF;
using DoubleGis.Erm.Qds.Etl.Transform.Docs;
using DoubleGis.Erm.Qds.Etl.Transform.EF;

using FluentAssertions;

using Machine.Specifications;

using Moq;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.AcceptanceTests
{
    class IndexationFacadeContext
    {
        public const string DocIdFieldName = "Id";

        Establish context = () =>
            {
                EnumLocalizer = new Mock<IEnumLocalizer>();
                DocsStorage = new MockDocsStorage();
                var queryDsl = new MockDocsStorage.QueryDsl();

                var trackerState = new DocsStorageChangesTrackerState(DocsStorage, queryDsl);
                DocsStorage.Add(new RecordIdState(0, 42), DocsStorageChangesTrackerState.IdFieldName, (long)0);

                var clientQds = new ClientGridDocQdsComponent(DocsStorage, EnumLocalizer.Object, queryDsl);
                var userQds = new UserDocQdsComponent(DocsStorage, queryDsl);
                var territoryQds = new TerritoryDocQdsComponent(DocsStorage, queryDsl);

                // TODO Сделать класс 
                var qdsFactory = new Mock<IQdsComponentsFactory>();
                qdsFactory.Setup(q => q.CreateQdsComponents()).Returns(new IQdsComponent[] { clientQds, userQds, territoryQds });

                Target = new IndexationFacade(DocsStorage, trackerState, qdsFactory.Object);
            };

        protected static Mock<IEnumLocalizer> EnumLocalizer { get; private set; }
        protected static IndexationFacade Target { get; private set; }
        protected static MockDocsStorage DocsStorage;

        protected static void AddToDocsStorage<TDoc>(TDoc doc, string fieldName, object value) where TDoc : IDoc
        {
            DocsStorage.Add(doc, fieldName, value);
        }

        protected static void PublishedDocsShouldContain<TDoc>(Func<TDoc, bool> checkDoc)
        {
            DocsStorage.NewPublishedDocs.Should()
                       .NotBeEmpty()
                       .And
                       .Contain(doc => doc is TDoc && checkDoc((TDoc)doc));
        }
    }
}