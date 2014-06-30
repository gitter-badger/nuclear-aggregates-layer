using System;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Qds.Common;
using DoubleGis.Erm.Qds.Docs;
using DoubleGis.Erm.Qds.Etl.Transform.EF;

namespace DoubleGis.Erm.Qds.Etl.Transform.Docs
{
    public class ClientGridDocQdsComponent : IQdsComponent
    {
        private readonly IRelationalDocsFinder _relationalDocsFinder;
        private readonly IDocumentRelationsRegistry _documentRelationsRegistry;

        public ClientGridDocQdsComponent(IDocsStorage docsStorage, IQueryDsl queryDsl, IDocumentRelationsRegistry documentRelationsRegistry)
        {
            if (docsStorage == null)
            {
                throw new ArgumentNullException("docsStorage");
            }

            if (queryDsl == null)
            {
                throw new ArgumentNullException("queryDsl");
            }

            if (documentRelationsRegistry == null)
            {
                throw new ArgumentNullException("documentRelationsRegistry");
            }

            _documentRelationsRegistry = documentRelationsRegistry;

            _relationalDocsFinder = new RelationalDocsFinder(this, docsStorage);
            IQueryDsl queryDsl1 = queryDsl;

            // FIXME {f.zaharov, 10.04.2014}: Нужно ввести ответственного за выдачу этих метаданных по типу документа
            PartsDocRelation = DocRelation.ForDoc<ClientGridDoc>()
                                          .LinkPart<Client>(new FieldsEqualsDocsQueryBuilder<ClientGridDoc, Client>(d => d.Id, c => c.Id, queryDsl1))
                                          .LinkPart<User>(new FieldsEqualsDocsQueryBuilder<ClientGridDoc, User>(d => d.OwnerCode, u => u.Id, queryDsl1))
                                          .LinkPart<Territory>(new FieldsEqualsDocsQueryBuilder<ClientGridDoc, Territory>(d => d.TerritoryId, t => t.Id, queryDsl1))

                                          .LinkPart<LegalPerson>(
                                              new OrDocsQueryBuilder(new NestedDocsQueryBuilder<ClientGridDoc>(c => c.LegalPersons,
                                                    new FieldsEqualsDocsQueryBuilder<LegalPersonDoc, LegalPerson>(d => d.Id, a => a.Id, queryDsl1), queryDsl1),
                                                    new FieldsEqualsDocsQueryBuilder<ClientGridDoc, LegalPerson>(d => d.Id, lp => lp.ClientId, queryDsl1), queryDsl1))

                                          .LinkPart<Account>(
                                              new OrDocsQueryBuilder(
                                                    new NestedDocsQueryBuilder<ClientGridDoc>(c => c.LegalPersons,
                                                        new NestedDocsQueryBuilder<LegalPersonDoc>(l => l.Accounts,
                                                            new FieldsEqualsDocsQueryBuilder<AccountDoc, Account>(d => d.Id, a => a.Id, queryDsl1), queryDsl1), queryDsl1),
                                                    new NestedDocsQueryBuilder<ClientGridDoc>(c => c.LegalPersons,
                                                        new FieldsEqualsDocsQueryBuilder<LegalPersonDoc, Account>(d => d.Id, a => a.LegalPersonId, queryDsl1), queryDsl1), queryDsl1));
        }

        // FIXME {f.zaharov, 09.04.2014}: ни у какого класса не должно быть ответственности создавать другой класс, для этого есть DI
        // FIXME {f.zaharov, 10.04.2014}: Отделить определение апдейтера документов на основе документа
        public IDocsUpdater CreateDocUpdater()
        {
            return new RelationalDocsUpdater<ClientGridDoc>(this, _relationalDocsFinder, new ClientGridDocMapper(_documentRelationsRegistry));
        }

        public IDocRelation PartsDocRelation { get; private set; }

        public IDoc CreateNewDoc(object part)
        {
            return (part is Client) ? new ClientGridDoc() : null;
        }
    }
}