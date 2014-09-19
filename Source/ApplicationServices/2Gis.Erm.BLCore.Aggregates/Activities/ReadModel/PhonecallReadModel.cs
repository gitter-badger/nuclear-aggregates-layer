using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
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

        public bool CheckIfRelatedActivitiesExists(EntityName entityName, long entityId)
        {
            return _finder.FindMany(Specs.Find.Custom<PhonecallRegardingObject>(x => x.TargetEntityName == entityName && x.TargetEntityId == entityId)).Any();
        }

        public bool CheckIfRelatedActiveActivitiesExists(EntityName entityName, long entityId)
        {
            // TODO {s.pomadin, 18.09.2014}: support other refeneces not only regarding objects
            var ids = (
                from reference in _finder.FindMany(Specs.Find.Custom<PhonecallRegardingObject>(x => x.TargetEntityName == entityName && x.TargetEntityId == entityId))
                select reference.SourceEntityId
                ).ToArray();

            return _finder.FindMany(Specs.Find.Custom<Phonecall>(x => x.Status == ActivityStatus.InProgress) & Specs.Find.ByIds<Phonecall>(ids)).Any();
        }

        public IEnumerable<Phonecall> LookupRelatedActivities(EntityName entityName, long entityId)
        {
            // TODO {s.pomadin, 18.09.2014}: support other refeneces not only regarding objects
            var ids = (
                from reference in _finder.FindMany(Specs.Find.Custom<PhonecallRegardingObject>(x => x.TargetEntityName == entityName && x.TargetEntityId == entityId))
                select reference.SourceEntityId
                ).ToArray();

            return _finder.FindMany(Specs.Find.Active<Phonecall>() && Specs.Find.ByIds<Phonecall>(ids));
        }
    }
}