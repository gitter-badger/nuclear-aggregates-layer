using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel
{
    public interface IPhonecallReadModel : IAggregateReadModel<Phonecall>
    {
        Phonecall GetPhonecall(long phonecallId);
        IEnumerable<PhonecallRegardingObject> GetRegardingObjects(long phonecallId);
        PhonecallRecipient GetRecipient(long phonecallId);

        bool CheckIfPhonecallExistsRegarding(EntityName entityName, long entityId);
        bool CheckIfOpenPhonecallExistsRegarding(EntityName entityName, long entityId);

        IEnumerable<Phonecall> LookupPhonecallsRegarding(EntityName entityName, long entityId);
        IEnumerable<Phonecall> LookupOpenPhonecallsRegarding(EntityName entityName, long entityId);
        IEnumerable<Phonecall> LookupOpenPhonecallsOwnedBy(long ownerCode);
    }
}