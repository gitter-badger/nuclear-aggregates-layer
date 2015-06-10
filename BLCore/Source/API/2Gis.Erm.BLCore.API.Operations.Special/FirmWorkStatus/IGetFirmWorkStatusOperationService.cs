using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.FirmWorkStatus
{
    public interface IGetFirmWorkStatusOperationService : IOperation<GetFirmWorkStatusIdentity>
    {
        FirmWorkStatus GetFirmWorkStatus(long firmId);
    }
}