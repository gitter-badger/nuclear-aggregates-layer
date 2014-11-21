using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Release;

namespace DoubleGis.Erm.BLCore.API.Releasing.Releases
{
    public interface IAttachExternalReleaseProcessingMessagesOperationService : IOperation<AttachExternalReleaseProcessingMessagesIdentity>
    {
        void Attach(long releaseId, ExternalReleaseProcessingMessage[] messages);
    }
}