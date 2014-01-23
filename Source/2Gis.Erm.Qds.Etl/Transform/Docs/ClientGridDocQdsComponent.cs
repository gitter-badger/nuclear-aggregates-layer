using System;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Qds.Etl.Transform.EF;

namespace DoubleGis.Erm.Qds.Etl.Transform.Docs
{
    public class ClientGridDocQdsComponent : IQdsComponent
    {
        private readonly IRelationalDocsFinder _relationalDocsFinder;
        private readonly IEnumLocalizer _enumLocalizer;

        public ClientGridDocQdsComponent(IDocsStorage docsStorage, IEnumLocalizer enumLocalizer, IQueryDsl queryDsl)
        {
            if (docsStorage == null)
            {
                throw new ArgumentNullException("docsStorage");
            }

            if (enumLocalizer == null)
            {
                throw new ArgumentNullException("enumLocalizer");
            }
            if (queryDsl == null)
            {
                throw new ArgumentNullException("queryDsl");
            }

            _relationalDocsFinder = new RelationalDocsFinder(this, docsStorage);
            _enumLocalizer = enumLocalizer;
            IQueryDsl queryDsl1 = queryDsl;

            // TODO сделать красиво!
            PartsDocRelation = DocRelation.ForDoc<ClientGridDoc>()
                                .LinkPart<Client>(new FieldsEqualsDocsQueryBuilder<ClientGridDoc, Client>(d => d.Id, c => c.Id, queryDsl1))
                                .LinkPart<User>(new FieldsEqualsDocsQueryBuilder<ClientGridDoc, User>(d => d.OwnerCode, u => u.Id, queryDsl1))
                                .LinkPart<Territory>(new FieldsEqualsDocsQueryBuilder<ClientGridDoc, Territory>(d => d.TerritoryId, t => t.Id, queryDsl1));

            IndirectDocRelations = new IDocRelation[]
                {
                    DocRelation.ForDoc<UserDoc>()
                                .LinkPart<Client>(new FieldsEqualsDocsQueryBuilder<UserDoc, Client>(d => d.Id, c => c.OwnerCode, queryDsl1)),

                    DocRelation.ForDoc<TerritoryDoc>()
                                .LinkPart<Client>(new FieldsEqualsDocsQueryBuilder<TerritoryDoc, Client>(d => d.Id, c => c.TerritoryId, queryDsl1)),
                };
        }

        public IDocRelation[] IndirectDocRelations { get; private set; }

        public IDocsSelector CreateDocSelector()
        {
            return new DocsSelector<ClientGridDoc>(this, _relationalDocsFinder, new ClientGridDocMapper(_enumLocalizer, _relationalDocsFinder));
        }

        public IDocRelation PartsDocRelation { get; private set; }

        public IDoc CreateNewDoc(object part)
        {
            return (part is Client) ? new ClientGridDoc() : null;
        }
    }
}