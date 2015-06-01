using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

using NuClear.Model.Common.Entities;
using NuClear.Storage.Readings;

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
            return _finder.Find(Specs.Find.ById<Phonecall>(phonecallId)).One();
        }

        public IEnumerable<PhonecallRegardingObject> GetRegardingObjects(long phonecallId)
        {
            return _finder.Find(Specs.Find.Custom<PhonecallRegardingObject>(x => x.SourceEntityId == phonecallId)).Many();
        }

        public PhonecallRecipient GetRecipient(long phonecallId)
        {
            return _finder.Find(Specs.Find.Custom<PhonecallRecipient>(x => x.SourceEntityId == phonecallId)).One();
        }

        public bool CheckIfPhonecallExistsRegarding(IEntityType entityName, long entityId)
        {
            return _finder.Find(ActivitySpecs.Find.ByReferencedObject<Phonecall, PhonecallRegardingObject>(entityName, entityId)).Any();
        }

        public bool CheckIfOpenPhonecallExistsRegarding(IEntityType entityName, long entityId)
        {
            var ids = (from reference in _finder.Find(ActivitySpecs.Find.ByReferencedObject<Phonecall, PhonecallRegardingObject>(entityName, entityId)).Many()
                       select reference.SourceEntityId)
                .ToArray();

            return _finder.Find(Specs.Find.Active<Phonecall>() &&
                                    Specs.Find.Custom<Phonecall>(x => x.Status == ActivityStatus.InProgress) &&
                                    Specs.Find.ByIds<Phonecall>(ids))
                          .Any();
        }

        public IEnumerable<Phonecall> LookupPhonecallsRegarding(IEntityType entityName, long entityId)
        {
            var ids = (from reference in _finder.Find(ActivitySpecs.Find.ByReferencedObject<Phonecall, PhonecallRegardingObject>(entityName, entityId)).Many()
                       select reference.SourceEntityId)
                .ToArray();

            return _finder.Find(Specs.Find.Active<Phonecall>() & Specs.Find.ByIds<Phonecall>(ids)).Many();
        }

        public IEnumerable<Phonecall> LookupOpenPhonecallsRegarding(IEntityType entityName, long entityId)
        {
            var ids = (from reference in _finder.Find(ActivitySpecs.Find.ByReferencedObject<Phonecall, PhonecallRegardingObject>(entityName, entityId)).Many()
                       select reference.SourceEntityId)
                .ToArray();

            return _finder.Find(Specs.Find.Active<Phonecall>() & Specs.Find.Custom<Phonecall>(x => x.Status == ActivityStatus.InProgress) & Specs.Find.ByIds<Phonecall>(ids)).Many();
        }

        public IEnumerable<Phonecall> LookupOpenPhonecallsOwnedBy(long ownerCode)
        {
            return _finder.Find(Specs.Find.Owned<Phonecall>(ownerCode) &
                                    Specs.Find.Custom<Phonecall>(x => x.Status == ActivityStatus.InProgress)).Many();
        }
    }
}