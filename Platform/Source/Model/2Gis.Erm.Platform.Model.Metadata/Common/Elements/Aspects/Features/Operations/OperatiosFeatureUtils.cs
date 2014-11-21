using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Operations
{
    public static class OperatiosFeatureUtils
    {
        public static TBuilder AddOperation<TBuilder, TMetadataElement>(this TBuilder builder, StrictOperationIdentity strictOperationIdentity)
            where TBuilder : MetadataElementBuilder<TBuilder, TMetadataElement>, new()
            where TMetadataElement : MetadataElement
        {
            var batchOperationFeature = builder.Features.OfType<OperationsSetFeature>().SingleOrDefault();
            if (batchOperationFeature == null)
            {
                batchOperationFeature = new OperationsSetFeature();
                builder.WithFeatures(batchOperationFeature);
            }

            batchOperationFeature.OperationFeatures.Add(new OperationFeature(strictOperationIdentity));
            return builder;
        }

        public static TBuilder AddEntitySpecificOperation<TBuilder, TMetadataElement, TOperationIdentity>(this TBuilder builder, params EntityName[] operationEntities)
            where TBuilder : MetadataElementBuilder<TBuilder, TMetadataElement>, new()
            where TMetadataElement : MetadataElement
            where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, IEntitySpecificOperationIdentity, new()
        {
            var identity = new TOperationIdentity();
            return builder.AddOperation<TBuilder, TMetadataElement>(identity.SpecificFor(operationEntities));
        }

        public static TBuilder AddEntitySpecificOperation<TBuilder, TMetadataElement, TOperationIdentity, TEntity>(this TBuilder builder)
            where TBuilder : MetadataElementBuilder<TBuilder, TMetadataElement>, new()
            where TMetadataElement : MetadataElement
            where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, IEntitySpecificOperationIdentity, new()
            where TEntity : class, IEntity
        {
            var identity = new TOperationIdentity();
            return builder.AddOperation<TBuilder, TMetadataElement>(identity.SpecificFor<TEntity>());
        }

        public static TBuilder AddEntitySpecificOperation<TBuilder, TMetadataElement, TOperationIdentity, TEntity1, TEntity2>(this TBuilder builder)
            where TBuilder : MetadataElementBuilder<TBuilder, TMetadataElement>, new()
            where TMetadataElement : MetadataElement
            where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, IEntitySpecificOperationIdentity, new()
            where TEntity1 : class, IEntity
            where TEntity2 : class, IEntity
        {
            var identity = new TOperationIdentity();
            return builder.AddOperation<TBuilder, TMetadataElement>(identity.SpecificFor<TEntity1, TEntity2>());
        }

        public static TBuilder AddNonCoupledOperation<TBuilder, TMetadataElement, TOperationIdentity>(this TBuilder builder)
            where TBuilder : MetadataElementBuilder<TBuilder, TMetadataElement>, new()
            where TMetadataElement : MetadataElement
            where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, INonCoupledOperationIdentity, new()
        {
            var identity = new TOperationIdentity();
            return builder.AddOperation<TBuilder, TMetadataElement>(identity.NonCoupled());
        }
    }
}
