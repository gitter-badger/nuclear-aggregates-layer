using System;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Qds.Etl.Transform.EF;

namespace DoubleGis.Erm.Qds.Etl.Transform.Docs
{
    public class TerritoryDocQdsComponent : IQdsComponent
    {
        readonly IRelationalDocsFinder _relationalDocsFinder;

        public TerritoryDocQdsComponent(IDocsStorage docsStorage, IQueryDsl queryDsl)
        {
            if (docsStorage == null)
            {
                throw new ArgumentNullException("docsStorage");
            }
            if (queryDsl == null)
            {
                throw new ArgumentNullException("queryDsl");
            }

            _relationalDocsFinder = new RelationalDocsFinder(this, docsStorage);

            PartsDocRelation = DocRelation.ForDoc<TerritoryDoc>()
                                     .LinkPart<Territory>(new FieldsEqualsDocsQueryBuilder<TerritoryDoc, Territory>(d => d.Id, u => u.Id, queryDsl));

            IndirectDocRelations = new IDocRelation[0];
        }

        public IDocsSelector CreateDocSelector()
        {
            return new DocsSelector<TerritoryDoc>(this, _relationalDocsFinder, new TerritoryDocMapper());
        }

        public IDocRelation PartsDocRelation { get; private set; }

        public IDocRelation[] IndirectDocRelations { get; private set; }

        public IDoc CreateNewDoc(object part)
        {
            return (part is Territory) ? new TerritoryDoc() : null;
        }
    }
}