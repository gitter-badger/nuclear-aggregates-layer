using System.Collections.Generic;

using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities
{
    public interface IUpdateLetterAggregateService : IAggregateSpecificOperation<Letter, UpdateIdentity>
    {
        void Update(Letter letter);

        void ChangeRegardingObjects(Letter letter,
                                    IEnumerable<LetterRegardingObject> oldReferences,
                                    IEnumerable<LetterRegardingObject> newReferences);

        void ChangeSender(Letter letter, LetterSender oldSender, LetterSender newSender);
        
        void ChangeRecipient(Letter letter, LetterRecipient oldRecipient, LetterRecipient newRecipient);
    }
}