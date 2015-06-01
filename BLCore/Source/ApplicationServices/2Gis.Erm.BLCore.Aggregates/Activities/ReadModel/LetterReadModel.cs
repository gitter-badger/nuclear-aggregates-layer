using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

using NuClear.Model.Common.Entities;
using NuClear.Storage.Readings;

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
            return _finder.Find(Specs.Find.ById<Letter>(letterId)).One();
        }

        public IEnumerable<LetterRegardingObject> GetRegardingObjects(long letterId)
        {
            return _finder.Find(Specs.Find.Custom<LetterRegardingObject>(x => x.SourceEntityId == letterId)).Many();
        }

        public LetterSender GetSender(long letterId)
        {
            return _finder.Find(Specs.Find.Custom<LetterSender>(x => x.SourceEntityId == letterId)).One();
        }

        public LetterRecipient GetRecipient(long letterId)
        {
            return _finder.Find(Specs.Find.Custom<LetterRecipient>(x => x.SourceEntityId == letterId)).One();
        }

        public bool CheckIfLetterExistsRegarding(IEntityType entityName, long entityId)
        {
            return _finder.Find(ActivitySpecs.Find.ByReferencedObject<Letter, LetterRegardingObject>(entityName, entityId)).Any();
        }

        public bool CheckIfOpenLetterExistsRegarding(IEntityType entityName, long entityId)
        {
            var ids = (from reference in _finder.Find(ActivitySpecs.Find.ByReferencedObject<Letter, LetterRegardingObject>(entityName, entityId)).Many()
                       select reference.SourceEntityId)
                .ToArray();

            return _finder.Find(Specs.Find.Active<Letter>() &&
                                    Specs.Find.Custom<Letter>(x => x.Status == ActivityStatus.InProgress) &&
                                    Specs.Find.ByIds<Letter>(ids))
                          .Any();
        }

        public IEnumerable<Letter> LookupLettersRegarding(IEntityType entityName, long entityId)
        {
            var ids = (from reference in _finder.Find(ActivitySpecs.Find.ByReferencedObject<Letter, LetterRegardingObject>(entityName, entityId)).Many()
                       select reference.SourceEntityId)
                .ToArray();

            return _finder.Find(Specs.Find.Active<Letter>() & Specs.Find.ByIds<Letter>(ids)).Many();
        }

        public IEnumerable<Letter> LookupOpenLettersRegarding(IEntityType entityName, long entityId)
        {
            var ids = (from reference in _finder.Find(ActivitySpecs.Find.ByReferencedObject<Letter, LetterRegardingObject>(entityName, entityId)).Many()
                       select reference.SourceEntityId)
                .ToArray();

            return _finder.Find(Specs.Find.Active<Letter>() & Specs.Find.Custom<Letter>(x => x.Status == ActivityStatus.InProgress) & Specs.Find.ByIds<Letter>(ids)).Many();
        }

        public IEnumerable<Letter> LookupOpenLettersOwnedBy(long ownerCode)
        {
            return _finder.Find(Specs.Find.Owned<Letter>(ownerCode) & Specs.Find.Custom<Letter>(x => x.Status == ActivityStatus.InProgress)).Many();
        }
    }
}