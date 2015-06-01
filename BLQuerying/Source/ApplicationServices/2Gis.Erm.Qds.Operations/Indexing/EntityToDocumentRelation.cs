using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Qds.API.Operations.Indexing;
using DoubleGis.Erm.Qds.API.Operations.Replication.Metadata.Features;

using FastMember;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Readings;
using NuClear.Storage.Readings.Queryable;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Qds.Operations.Indexing
{
    public sealed class EntityToDocumentRelation<TEntity, TDocument> : IEntityToDocumentRelation<TEntity, TDocument>
        where TEntity : class, IEntity, IEntityKey
    {
        private readonly IFinder _finder;
        private readonly SelectSpecification<TEntity, object> _selectSpec;
        private readonly IMapSpecification<ObjectAccessor, IIndexedDocumentWrapper> _mapSpec;

        public EntityToDocumentRelation(IFinder finder, 
                                        EntityRelationFeature<TDocument, TEntity> entityRelationFeature)
        {
            _finder = finder;
            _selectSpec = entityRelationFeature.SelectSpec;
            _mapSpec = entityRelationFeature.MapSpec;
        }

        public IEnumerable<IIndexedDocumentWrapper> SelectAllDocuments(IProgress<long> progress = null)
        {
            if (progress != null)
            {
                var totalCount = _finder.Find(Specs.Find.Custom<TEntity>(x => true)).Fold(q => q.LongCount());
                progress.Report(totalCount);
            }

            return SelectDocuments(Specs.Find.Custom<TEntity>(x => true));
        }

        public IEnumerable<IIndexedDocumentWrapper> SelectDocuments(IReadOnlyCollection<long> ids)
        {
            return SelectDocuments(Specs.Find.ByIds<TEntity>(ids));
        }

        private IEnumerable<IIndexedDocumentWrapper> SelectDocuments(FindSpecification<TEntity> findSpec)
        {
            var entities = _finder.Find(findSpec).Map(q => q.Select(_selectSpec)).Many();
            return entities.Select(x => _mapSpec.Map(ObjectAccessor.Create(x)));
        }
    }
}