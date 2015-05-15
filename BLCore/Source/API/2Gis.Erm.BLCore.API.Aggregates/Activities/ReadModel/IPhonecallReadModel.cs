using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel
{
    public interface IPhonecallReadModel : IAggregateReadModel<Phonecall>
    {
        Phonecall GetPhonecall(long phonecallId);
        IEnumerable<PhonecallRegardingObject> GetRegardingObjects(long phonecallId);
        PhonecallRecipient GetRecipient(long phonecallId);

        bool CheckIfPhonecallExistsRegarding(IEntityType entityName, long entityId);
        bool CheckIfOpenPhonecallExistsRegarding(IEntityType entityName, long entityId);

        IEnumerable<Phonecall> LookupPhonecallsRegarding(IEntityType entityName, long entityId);
        IEnumerable<Phonecall> LookupOpenPhonecallsRegarding(IEntityType entityName, long entityId);
        IEnumerable<Phonecall> LookupOpenPhonecallsOwnedBy(long ownerCode);
    }
}