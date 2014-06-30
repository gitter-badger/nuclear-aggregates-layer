using System;

using DoubleGis.Erm.BLQuerying.DI;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Qds.Common;
using DoubleGis.Erm.Qds.Etl.Extract.EF;
using DoubleGis.Erm.Qds.Etl.Transform.Docs;

using FluentAssertions;

using Machine.Specifications;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.AcceptanceTests
{
    class IndexationFacadeContext
    {
        public const string DocIdFieldName = "id";

        Establish context = () =>
            {
                DocsStorage = new MockDocsStorage();
                DocsStorage.Add(new RecordIdState("0", "42"), DocIdFieldName, "0");

                var container = new UnityContainer();

                container.RegisterInstance<IEnumLocalizer>(new ResourcesEnumLocalizer());
                container.RegisterInstance<IDocsStorage>(DocsStorage);
                container.RegisterInstance<IElasticApi>(DocsStorage); // FIXME {m.pashuk, 29.04.2014}: По задумке тут не должно быть зависимости от Elastic. IDocsStorage - это и есть та грань которая отделяет индексацию от реального мира

                container.RegisterType<IQueryDsl, MockDocsStorage.CursorQueryDsl>();
                container.RegisterType<IQdsComponentsFactory, UnityQdsComponentsFactory>();
                container.RegisterType<IDocumentRelationsRegistry>(Lifetime.Singleton, new InjectionFactory(x => new UnityDocumentRelationsRegistry(x).RegisterAllDocumentParts(() => Lifetime.Singleton)));

                Target = container.Resolve<IndexationFacade>();
            };

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

        protected static IndexationFacade Target { get; private set; }
        protected static MockDocsStorage DocsStorage { get; private set; }
    }
}