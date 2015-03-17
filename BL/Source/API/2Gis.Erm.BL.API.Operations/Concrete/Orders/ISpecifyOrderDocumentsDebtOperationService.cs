using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BL.API.Operations.Concrete.Orders
{
    public interface ISpecifyOrderDocumentsDebtOperationService : IOperation<SpecifyOrderDocumentsDebtIdentity>
    {
        void Specify(long orderId, DocumentsDebt documentsDebt, string documentsDebtComment);
    }
}