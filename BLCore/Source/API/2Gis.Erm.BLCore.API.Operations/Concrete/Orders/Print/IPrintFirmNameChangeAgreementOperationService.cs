using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders.Print
{
    public interface IPrintFirmNameChangeAgreementOperationService : IOperation<PrintFirmNameChangeAgreementIdentity>
    {
        PrintFormDocument Print(long orderId);
    }
}