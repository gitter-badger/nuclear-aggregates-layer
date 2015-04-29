using System.Collections.Generic;

using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel
{
    public interface ILetterReadModel : IAggregateReadModel<Letter>
    {
        Letter GetLetter(long letterId);
        IEnumerable<LetterRegardingObject> GetRegardingObjects(long letterId);
        LetterSender GetSender(long letterId);
        LetterRecipient GetRecipient(long letterId);

        bool CheckIfLetterExistsRegarding(IEntityType entityName, long entityId);
        bool CheckIfOpenLetterExistsRegarding(IEntityType entityName, long entityId);

        IEnumerable<Letter> LookupLettersRegarding(IEntityType entityName, long entityId);
        IEnumerable<Letter> LookupOpenLettersRegarding(IEntityType entityName, long entityId);
        IEnumerable<Letter> LookupOpenLettersOwnedBy(long ownerCode);
    }
}