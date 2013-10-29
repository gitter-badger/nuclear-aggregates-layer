using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Operations
{
    public sealed class OperationBoundFeatureAspect<TBuilder, TConfigElement> : ConfigElementBuilderAspectBase<TBuilder, IOperationsBoundElement, TConfigElement>
        where TBuilder : ConfigElementBuilder<TBuilder, TConfigElement>, new()
        where TConfigElement : ConfigElement, IOperationsBoundElement
    {
        public OperationBoundFeatureAspect(ConfigElementBuilder<TBuilder, TConfigElement> aspectHostBuilder)
            : base(aspectHostBuilder)
        {
        }

        public TBuilder EntitySpecific<TOperationIdentity>(params EntityName[] entities)
            where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, new()
        {
            return AspectHostBuilder.AddOperation<TBuilder, TConfigElement>(
                new EntitySpecificOperationFeature<TOperationIdentity>
                {
                    Entity = entities.ToEntitySet()
                });
        }

        public TBuilder NonCoupled<TOperationIdentity>()
            where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, new()
        {
            return AspectHostBuilder.AddOperation<TBuilder, TConfigElement>(new NonCoupledOperationFeature<TOperationIdentity>());
        }
    }
}