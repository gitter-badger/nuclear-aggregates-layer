using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Cards;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import.Operations
{
    public interface IImportFirmService : IImportServiceBusDtoService<FirmServiceBusDto>, IOperation<ImportFirmIdentity>
    {
    }
}