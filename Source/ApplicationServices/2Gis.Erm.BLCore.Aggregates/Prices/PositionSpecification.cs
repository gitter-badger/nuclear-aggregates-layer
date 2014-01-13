using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices
{
    public class PositionSpecification
    {
        public static IFindSpecification<PositionChildren> FindUsedAsChildElement(int positionId)
        {
            return new FindSpecification<PositionChildren>(x => !x.IsDeleted && x.ChildPositionId == positionId);
        }
    }
}