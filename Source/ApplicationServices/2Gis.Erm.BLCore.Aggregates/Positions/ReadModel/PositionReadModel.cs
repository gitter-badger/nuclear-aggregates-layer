using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Positions.ReadModel;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Positions.ReadModel
{
    public partial class PositionReadModel : IPositionReadModel
    {
        private readonly IFinder _finder;

        public PositionReadModel(IFinder finder)
        {
            _finder = finder;
        }

        public PositionBindingObjectType GetPositionBindingObjectType(long positionId)
        {
            return _finder.Find(Specs.Find.ById<Position>(positionId)).Select(x => x.BindingObjectTypeEnum).Single();
        }

        public bool IsSupportedByExport(long positionId)
        {
            return _finder.Find(Specs.Find.ById<Position>(positionId)).Select(x => x.Platform.IsSupportedByExport).SingleOrDefault();
        }

        public bool PositionsExist(IReadOnlyCollection<long> positionIds, out string message)
        {
            var existingPositionIds = _finder.Find(Specs.Find.ActiveAndNotDeleted<Position>() &&
                                                   Specs.Find.ByIds<Position>(positionIds))
                                             .Select(x => x.Id)
                                             .ToArray();

            var missingPositionIds = positionIds.Except(existingPositionIds).ToArray();
            if (missingPositionIds.Any())
            {
                message = string.Format("The following positions could not be found: {0}.", string.Join(", ", missingPositionIds));
                return false;
            }

            message = null;
            return true;
        }
    }
}