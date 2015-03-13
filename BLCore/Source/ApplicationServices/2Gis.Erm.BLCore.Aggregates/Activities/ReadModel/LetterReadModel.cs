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

        public bool CheckIfLetterExistsRegarding(EntityName entityName, long entityId)
        {
            return _finder.FindMany(ActivitySpecs.Find.ByReferencedObject<Letter, LetterRegardingObject>(entityName, entityId)).Any();
        }

        public bool CheckIfOpenLetterExistsRegarding(EntityName entityName, long entityId)
        {
            var ids = (from reference in _finder.FindMany(ActivitySpecs.Find.ByReferencedObject<Letter, LetterRegardingObject>(entityName, entityId))
                       select reference.SourceEntityId)
                .ToArray();

            return _finder.FindMany(Specs.Find.Active<Letter>() &&
                                    Specs.Find.Custom<Letter>(x => x.Status == ActivityStatus.InProgress) &&
                                    Specs.Find.ByIds<Letter>(ids))
                          .Any();
        }

        public IEnumerable<Letter> LookupLettersRegarding(EntityName entityName, long entityId)
        {
            var ids = (from reference in _finder.FindMany(ActivitySpecs.Find.ByReferencedObject<Letter, LetterRegardingObject>(entityName, entityId))
                       select reference.SourceEntityId)
                .ToArray();

            return _finder.FindMany(Specs.Find.Active<Letter>() & Specs.Find.ByIds<Letter>(ids)).ToArray();
        }

        public IEnumerable<Letter> LookupOpenLettersRegarding(EntityName entityName, long entityId)
        {
            var ids = (from reference in _finder.FindMany(ActivitySpecs.Find.ByReferencedObject<Letter, LetterRegardingObject>(entityName, entityId))
                       select reference.SourceEntityId)
                .ToArray();

            return _finder.FindMany(Specs.Find.Active<Letter>() & Specs.Find.Custom<Letter>(x => x.Status == ActivityStatus.InProgress) & Specs.Find.ByIds<Letter>(ids)).ToArray();
        }

        public IEnumerable<long> LookupOpenLettersOwnedBy(long ownerCode)
        {
            return _finder.FindMany(Specs.Find.Owned<Letter>(ownerCode) & Specs.Find.Custom<Letter>(x => x.Status == ActivityStatus.InProgress)).Select(s => s.Id).ToArray();
        }
    }
}