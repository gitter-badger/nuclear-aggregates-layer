using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.LocalMessages.DTO;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;

namespace DoubleGis.Erm.BLCore.API.Aggregates.LocalMessages
{
    public interface ILocalMessageRepository : IAggregateRootService<LocalMessage>
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