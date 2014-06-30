using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Qds.Etl.Transform.Docs
{
    public class RelationalDocsFinder : IRelationalDocsFinder
    {
        private readonly IQdsComponent _qdsComponent;
        private readonly IDocsStorage _docsStorage;

        public RelationalDocsFinder(IQdsComponent qdsComponent, IDocsStorage docsStorage)
        {
            if (qdsComponent == null)
            {
                throw new ArgumentNullException("qdsComponent");
            }
            if (docsStorage == null)
            {
                throw new ArgumentNullException("docsStorage");
            }

            _qdsComponent = qdsComponent;
            _docsStorage = docsStorage;
        }

        public IEnumerable<TDoc> FindDocsByRelatedPart<TDoc>(IEntityKey part) where TDoc : class, IDoc
        {
            if (part == null)
            {
                throw new ArgumentNullException("part");
            }

            var relation = _qdsComponent.PartsDocRelation;

            var docsQuery = relation.GetByPartQuery(part);

            return _docsStorage.Find<TDoc>(docsQuery);
        }
    }
}