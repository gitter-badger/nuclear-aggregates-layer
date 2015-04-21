using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Position;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Positions
{
    public interface IChangePositionSortingOrderOperationService : IOperation<ChangePositionSortingOrderIdentity>
    {
        void ApplyChanges(PositionSortingOrderDto[] data);
    }
}