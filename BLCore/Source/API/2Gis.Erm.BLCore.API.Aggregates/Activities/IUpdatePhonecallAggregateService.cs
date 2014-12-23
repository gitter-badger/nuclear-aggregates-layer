using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities
{
    public interface IUpdatePhonecallAggregateService : IAggregateSpecificOperation<Phonecall, UpdateIdentity>
    {
        void Update(Phonecall phonecall);

        void ChangeRegardingObjects(Phonecall phonecall,
                                    IEnumerable<PhonecallRegardingObject> oldReferences,
                                    IEnumerable<PhonecallRegardingObject> newReferences);

        void ChangeRecipient(Phonecall phonecall, PhonecallRecipient oldRecipient, PhonecallRecipient newRecipient);
    }
}