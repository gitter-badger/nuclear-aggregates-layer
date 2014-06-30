using System;

using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Qds.Common;
using DoubleGis.Erm.Qds.Docs;
using DoubleGis.Erm.Qds.Etl.Transform.EF;

namespace DoubleGis.Erm.Qds.Etl.Transform.Docs
{
    public class UserDocQdsComponent : IQdsComponent
    {
        private readonly IRelationalDocsFinder _relationalDocsFinder;
        private readonly IDocumentRelationsRegistry _documentRelationsRegistry;

        public UserDocQdsComponent(IDocsStorage docsStorage, IQueryDsl queryDsl, IDocumentRelationsRegistry documentRelationsRegistry)
        {
            _documentRelationsRegistry = documentRelationsRegistry;
            if (docsStorage == null)
            {
                throw new ArgumentNullException("docsStorage");
            }
            if (queryDsl == null)
            {
                throw new ArgumentNullException("queryDsl");
            }

            _relationalDocsFinder = new RelationalDocsFinder(this, docsStorage);

            PartsDocRelation = DocRelation.ForDoc<UserDoc>()
                           .LinkPart<User>(new FieldsEqualsDocsQueryBuilder<UserDoc, User>(d => d.Id, u => u.Id, queryDsl));
        }

        public IDocsUpdater CreateDocUpdater()
        {
            return new RelationalDocsUpdater<UserDoc>(this, _relationalDocsFinder, new UserDocMapper());
        }

        public IDocRelation PartsDocRelation { get; private set; }

        public IDoc CreateNewDoc(object part)
        {
            return (part is User) ? new UserDoc() : null;
        }
    }
}