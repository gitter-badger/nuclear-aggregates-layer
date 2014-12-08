using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Handler.Concrete
{
    public sealed class RequestHandlerFeature : IHandlerFeature
    {
        public RequestHandlerFeature(IResourceDescriptor requestDescriptor)
        {
            RequestDescriptor = requestDescriptor;
        }

        public IResourceDescriptor RequestDescriptor { get; private set; }
    }
}