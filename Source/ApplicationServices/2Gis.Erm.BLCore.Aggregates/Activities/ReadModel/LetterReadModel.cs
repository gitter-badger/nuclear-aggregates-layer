using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities.ReadModel
{
    public sealed class LetterReadModel : ILetterReadModel
    {
        private readonly IFinder _finder;

        public LetterReadModel(IFinder finder)
        {
            _finder = finder;
        }

        public Letter GetLetter(long letterId)
        {
            return _finder.FindOne(Specs.Find.ById<Letter>(letterId));
        }

        public IEnumerable<LetterRegardingObject> GetRegardingObjects(long letterId)
        {
            return _finder.FindMany(Specs.Find.Custom<LetterRegardingObject>(x => x.SourceEntityId == letterId)).ToList();
        }

        public LetterSender GetSender(long letterId)
        {
            return _finder.FindOne(Specs.Find.Custom<LetterSender>(x => x.SourceEntityId == letterId));
        }

        public LetterRecipient GetRecipient(long letterId)
        {
            return _finder.FindOne(Specs.Find.Custom<LetterRecipient>(x => x.SourceEntityId == letterId));
        }

        public bool CheckIfRelatedActivitiesExists(EntityName entityName, long entityId)
        {
            return _finder.FindMany(Specs.Find.Custom<LetterRegardingObject>(x => x.TargetEntityName == entityName && x.TargetEntityId == entityId)).Any();
        }

        public bool CheckIfRelatedActiveActivitiesExists(EntityName entityName, long entityId)
        {
            return (
                from letter in _finder.FindMany(Specs.Find.Custom<Letter>(x => x.Status == ActivityStatus.InProgress))
                join regardingObject in _finder.FindMany(Specs.Find.Custom<LetterRegardingObject>(x => x.TargetEntityName == entityName && x.TargetEntityId == entityId))
                    on letter.Id equals regardingObject.SourceEntityId
                select letter
                ).Any();
        }
    }
}