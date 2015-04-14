using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Handler.Concrete;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Operations;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Handler
{
    public sealed class HandlerFeatureAspect<TBuilder, TMetadataElement> : MetadataElementBuilderAspectBase<TBuilder, IHandlerBoundElement, TMetadataElement>
        where TBuilder : MetadataElementBuilder<TBuilder, TMetadataElement>, new()
        where TMetadataElement : MetadataElement, IHandlerBoundElement
    {
        public HandlerFeatureAspect(MetadataElementBuilder<TBuilder, TMetadataElement> aspectHostBuilder)
            : base(aspectHostBuilder)
        {
        }

        public TBuilder Use(IHandlerFeature feature)
        {
            AspectHostBuilder.WithFeatures(feature);
            return AspectHostBuilder;
        }

        public TBuilder ShowGridByConvention(EntityName entityName)
        {
            AspectHostBuilder.WithFeatures(new ShowGridHandlerFeature(entityName));
            return AspectHostBuilder.AddOperation<TBuilder, TMetadataElement>(ListIdentity.Instance.SpecificFor(entityName));
        }

        public TBuilder Name(string handlerName)
        {
            AspectHostBuilder.WithFeatures(new NamedHandlerFeature(handlerName));
            return AspectHostBuilder;
        }

        public TBuilder Request(string request)
        {
            AspectHostBuilder.WithFeatures(new RequestHandlerFeature(new StaticStringResourceDescriptor(request)));
            return AspectHostBuilder;
        }
    }
}