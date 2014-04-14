using System.Linq;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Positions.ReadModel
{
    public class PositionReadModel : IPositionReadModel
    {
        private readonly IFinder _finder;

        public PositionReadModel(IFinder finder)
        {
            _finder = finder;
        }

        public PositionBindingObjectType GetPositionBindingObjectType(long positionId)
        {
            return (PositionBindingObjectType)_finder.Find(Specs.Find.ById<Position>(positionId)).Select(x => x.BindingObjectTypeEnum).Single();
        }

        public bool IsSupportedByExport(long positionId)
        {
            return _finder.Find(Specs.Find.ById<Position>(positionId)).Select(x => x.Platform.IsSupportedByExport).SingleOrDefault();
        }
    }
}