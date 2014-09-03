using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders
{
    // FIXME {a.rechkalov, 03.09.2014}: Этот сервис нигде не используется
    public interface ISelectPrintProfilesOperationService : IOperation<SelectPrintProfilesIdentity>
    {
        OrderProfiles SelectProfilesByBill(long billId);
        OrderProfiles SelectProfilesByOrder(long orderId);
    }

    public sealed class OrderProfiles
    {
        public long LegalPersonProfileId { get; set; }
    }
}
