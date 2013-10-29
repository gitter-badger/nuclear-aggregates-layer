using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Handler.Concrete;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Operations;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Handler
{
    public sealed class HandlerFeatureAspect<TBuilder, TConfigElement> : ConfigElementBuilderAspectBase<TBuilder, IHandlerBoundElement, TConfigElement>
        where TBuilder : ConfigElementBuilder<TBuilder, TConfigElement>, new()
        where TConfigElement : ConfigElement, IHandlerBoundElement
    {
        public HandlerFeatureAspect(ConfigElementBuilder<TBuilder, TConfigElement> aspectHostBuilder)
            : base(aspectHostBuilder)
        {
        }

        public TBuilder Use(IHandlerFeature feature)
        {
            AspectHostBuilder.Features.Add(feature);
            return AspectHostBuilder;
        }

        public TBuilder ShowGridByConvention(EntityName entityName, string filterExpression, string disableExpression)
        {
            AspectHostBuilder.Features.Add(new ShowGridHandlerFeature(entityName)
                {
                    FilterExpression = filterExpression, 
                    DisableExpression = disableExpression
                });

            return AspectHostBuilder.AddOperation<TBuilder, TConfigElement>(
                new EntitySpecificOperationFeature<ListIdentity>
                {
                    Entity = entityName.ToEntitySet()
                });
        }
    }
}
