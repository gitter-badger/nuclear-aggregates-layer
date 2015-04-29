using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Releasing.Releases;
using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Releases.Operations
{
    public interface IReleaseAttachProcessingMessagesAggregateService : IAggregatePartRepository<ReleaseInfo>
    {
        void SaveInternalMessages(ReleaseInfo release, IEnumerable<ReleaseProcessingMessage> messages);
        void SaveExternalMessages(ReleaseInfo release, IEnumerable<ExternalReleaseProcessingMessage> messages);
    }
}