using System.Collections.Generic;

using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities
{
    public interface IUpdatePhonecallAggregateService : IAggregateSpecificService<Phonecall, UpdateIdentity>
    {
        void Update(Phonecall phonecall);

        void ChangeRegardingObjects(Phonecall phonecall,
                                    IEnumerable<PhonecallRegardingObject> oldReferences,
                                    IEnumerable<PhonecallRegardingObject> newReferences);

        void ChangeRecipient(Phonecall phonecall, PhonecallRecipient oldRecipient, PhonecallRecipient newRecipient);
    }
}