using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BL.API.Operations.Concrete.Orders
{
    public interface ISetOrderDocumentsDebtOperationService : IOperation<SetOrderDocumentsDebtIdentity>
    {
        void Set(long orderId, DocumentsDebt documentsDebt, string documentsDebtComment);
    }
}