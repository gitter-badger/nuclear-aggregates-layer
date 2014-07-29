using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Billing;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Charge;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import.Operations
{
    public interface IImportChargesInfoService : IImportServiceBusDtoService<ChargesInfoServiceBusDto>, IOperation<ImportChargesInfoIdentity>
    {
    }
}