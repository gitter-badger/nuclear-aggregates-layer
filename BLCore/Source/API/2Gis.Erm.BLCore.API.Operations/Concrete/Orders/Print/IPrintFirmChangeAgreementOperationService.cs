using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders.Print
{
    public interface IPrintFirmChangeAgreementOperationService : IOperation<PrintFirmChangeAgreementIdentity>
    {
        PrintFormDocument Print(long orderId);
    }
}