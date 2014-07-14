using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Qds.Common;

namespace DoubleGis.Erm.Elastic.Nest.Qds.Indexing
{
    public class DefaultEntityToDocumentRelation<TEntity, TDocument> : IEntityToDocumentRelation<TEntity, TDocument>
        where TEntity : class, IEntity, IEntityKey
    {
        private readonly IFinder _finder;
        private readonly IElasticApi _elasticApi;
        private readonly IDocumentRelation<TDocument>[] _documentRelations;

        public DefaultEntityToDocumentRelation(IFinder finder, IElasticApi elasticApi, IDocumentRelationFactory documentRelationFactory)
        {
            _finder = finder;
            _elasticApi = elasticApi;
            _documentRelations = documentRelationFactory.GetDocumentRelations<TDocument>().ToArray();
        }

        public Func<IQueryable<TEntity>, IEnumerable<IDocumentWrapper<TDocument>>> SelectDocumentsFunc { private get; set; }

        public IEnumerable<IDocumentWrapper<TDocument>> SelectAllDocuments()
        {
            return SelectDocuments(q => q);
        }

        public IEnumerable<IDocumentWrapper<TDocument>> SelectDocuments(ICollection<long> ids)
        {
            return SelectDocuments(q => q.Where(x => ids.Contains(x.Id)));
        }

        private IEnumerable<IDocumentWrapper<TDocument>> SelectDocuments(Func<IQueryable<TEntity>,IQueryable<TEntity>> querySelector)
        {
            var query = _finder.FindAll<TEntity>();
            query = querySelector(query);

            var documentWrappers = SelectDocumentsFunc(query);
            var batches = _elasticApi.CreateBatches(documentWrappers);

            return batches.SelectMany(batch =>
            {
                foreach (var documentRelation in _documentRelations)
                {
                    documentRelation.UpdateDocumentParts(batch);
                }

                return batch;
            });
        }
    }
}
