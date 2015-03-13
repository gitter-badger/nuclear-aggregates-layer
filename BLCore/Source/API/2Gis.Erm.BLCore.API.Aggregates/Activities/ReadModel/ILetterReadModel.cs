using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel
{
    public interface ILetterReadModel : IAggregateReadModel<Letter>
    {
        Letter GetLetter(long letterId);
        IEnumerable<LetterRegardingObject> GetRegardingObjects(long letterId);
        LetterSender GetSender(long letterId);
        LetterRecipient GetRecipient(long letterId);

        bool CheckIfLetterExistsRegarding(EntityName entityName, long entityId);
        bool CheckIfOpenLetterExistsRegarding(EntityName entityName, long entityId);

        IEnumerable<Letter> LookupLettersRegarding(EntityName entityName, long entityId);
        IEnumerable<Letter> LookupOpenLettersRegarding(EntityName entityName, long entityId);
        IEnumerable<long> LookupOpenLettersOwnedBy(long ownerCode);
    }
}