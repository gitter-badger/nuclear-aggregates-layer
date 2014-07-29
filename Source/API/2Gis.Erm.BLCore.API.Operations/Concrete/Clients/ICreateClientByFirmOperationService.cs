using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Client;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Clients
{
    public interface ICreateClientByFirmOperationService : IOperation<CreateClientByFirmIdentity>
    {
        Client CreateByFirm(Firm firm, long ownerCode);
    }
}
