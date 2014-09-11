using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel
{
    public interface IPhonecallReadModel : IAggregateReadModel<Phonecall>
    {
        Phonecall GetPhonecall(long phonecallId);

        IEnumerable<PhonecallRegardingObject> GetRegardingObjects(long phonecallId);

        PhonecallRecipient GetRecipient(long phonecallId);

        bool CheckIfRelatedActivitiesExists(long clientId);
    }
}