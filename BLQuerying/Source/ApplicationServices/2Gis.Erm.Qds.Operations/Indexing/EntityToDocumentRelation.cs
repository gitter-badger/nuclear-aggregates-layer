using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Qds.API.Operations.Indexing;
using DoubleGis.Erm.Qds.API.Operations.Replication.Metadata.Features;

using FastMember;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Qds.Operations.Indexing
{
    public sealed class EntityToDocumentRelation<TEntity, TDocument> : IEntityToDocumentRelation<TEntity, TDocument>
        where TEntity : class, IEntity, IEntityKey
    {
        private readonly IFinder _finder;
        private readonly ISelectSpecification<TEntity, object> _selectSpec;
        private readonly IProjectSpecification<ObjectAccessor, IIndexedDocumentWrapper> _projectSpec;

        public EntityToDocumentRelation(IFinder finder,
                                        EntityRelationFeature<TDocument, TEntity> entityRelationFeature)
        {
            _finder = finder;
            _selectSpec = entityRelationFeature.SelectSpec;
            _projectSpec = entityRelationFeature.ProjectSpec;
        }

        public IEnumerable<IIndexedDocumentWrapper> SelectAllDocuments(IProgress<long> progress = null)
        {
            if (progress != null)
            {
                var totalCount = _finder.Find(Specs.Find.Custom<TEntity>(x => true)).LongCount();
                progress.Report(totalCount);
            }

            return SelectDocuments(Specs.Find.Custom<TEntity>(x => true));
        }

        public IEnumerable<IIndexedDocumentWrapper> SelectDocuments(IReadOnlyCollection<long> ids)
        {
            return SelectDocuments(Specs.Find.ByIds<TEntity>(ids));
        }

        private IEnumerable<IIndexedDocumentWrapper> SelectDocuments(IFindSpecification<TEntity> findSpec)
        {
            var entities = _finder.Find(_selectSpec, findSpec).AsEnumerable();
            return entities.Select(x => _projectSpec.Project(ObjectAccessor.Create(x)));
        }
    }
}