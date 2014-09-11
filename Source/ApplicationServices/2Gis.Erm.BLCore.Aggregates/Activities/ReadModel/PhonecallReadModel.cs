using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities.ReadModel
{
    public sealed class PhonecallReadModel : IPhonecallReadModel
    {
        private readonly IFinder _finder;

        public PhonecallReadModel(IFinder finder)
        {
            _finder = finder;
        }

        public Phonecall GetPhonecall(long phonecallId)
        {
            return _finder.FindOne(Specs.Find.ById<Phonecall>(phonecallId));
        }

        public IEnumerable<PhonecallRegardingObject> GetRegardingObjects(long phonecallId)
        {
            return _finder.FindMany(Specs.Find.Custom<PhonecallRegardingObject>(x => x.SourceEntityId == phonecallId)).ToList();
        }

        public PhonecallRecipient GetRecipient(long phonecallId)
        {
            return _finder.FindOne(Specs.Find.Custom<PhonecallRecipient>(x => x.SourceEntityId == phonecallId));
        }

        public bool CheckIfRelatedActivitiesExists(long clientId)
        {
            return _finder.FindMany(Specs.Find.Custom<PhonecallRegardingObject>(x => x.TargetEntityId == clientId)).Any();
        }
    }
}