using System;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Qds.Docs;
using DoubleGis.Erm.Qds.Etl.Transform.EF;

namespace DoubleGis.Erm.Qds.Etl.Transform.Docs
{
    public class OrderGridDocQdsComponent : IQdsComponent
    {
        readonly RelationalDocsFinder _relationalDocsFinder;
        readonly IEnumLocalizer _enumLocalizer;

        public OrderGridDocQdsComponent(IDocsStorage docsStorage, IQueryDsl queryDsl, IEnumLocalizer enumLocalizer)
        {
            _enumLocalizer = enumLocalizer;
            if (docsStorage == null)
            {
                throw new ArgumentNullException("docsStorage");
            }

            if (queryDsl == null)
            {
                throw new ArgumentNullException("queryDsl");
            }
            if (enumLocalizer == null)
            {
                throw new ArgumentNullException("enumLocalizer");
            }

            _relationalDocsFinder = new RelationalDocsFinder(this, docsStorage);

            IQueryDsl queryDsl1 = queryDsl;

            PartsDocRelation = DocRelation.ForDoc<OrderGridDoc>()
                                          .LinkPart<Order>(new FieldsEqualsDocsQueryBuilder<OrderGridDoc, Order>(d => d.Id, c => c.Id, queryDsl1));
        }

        public IDocsUpdater CreateDocUpdater()
        {
            return new RelationalDocsUpdater<OrderGridDoc>(this, _relationalDocsFinder, new OrderGridDocMapper(_enumLocalizer));
        }

        public IDocRelation PartsDocRelation { get; private set; }

        public IDoc CreateNewDoc(object part)
        {
            return (part is Order) ? new OrderGridDoc() : null;
        }
    }
}