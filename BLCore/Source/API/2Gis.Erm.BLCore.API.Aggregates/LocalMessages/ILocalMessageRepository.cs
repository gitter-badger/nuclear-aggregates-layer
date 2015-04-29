using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.LocalMessages.DTO;
using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.LocalMessages
{
    public interface ILocalMessageRepository : IAggregateRootRepository<LocalMessage>
    {
        void Delete(LocalMessage localMessage);
        void Create(LocalMessage localMessage, int integrationType); // тут пока не мен€ю на long // ј вдруг уже пора?

        IEnumerable<LocalMessage> GetByIds(long[] ids);
        IEnumerable<LocalMessage> GetLongProcessingMessages(int periodInMinutes);

        void SetResult(LocalMessage localMessage, LocalMessageStatus localMessageStatus, IEnumerable<string> messages, long processingTime);

        void SetProcessingState(LocalMessage localMessage);
        LocalMessageDto GetMessageToProcess();

        void SetWaitForProcessState(long localMessageId);
    }
}