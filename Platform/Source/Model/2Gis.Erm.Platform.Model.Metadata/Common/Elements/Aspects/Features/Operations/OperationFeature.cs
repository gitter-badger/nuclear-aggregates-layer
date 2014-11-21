using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Operations
{
    public sealed class OperationFeature : IMetadataFeature
    {
        public OperationFeature(StrictOperationIdentity strictOperationIdentity)
        {
            Identity = strictOperationIdentity;
        }

        public StrictOperationIdentity Identity { get; private set; }
    }
}