using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Stickers;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.HotClientRequest;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import.Operations
{
    public interface IImportHotClientService : IImportServiceBusDtoService<HotClientServiceBusDto>, IOperation<ImportHotClientIdentity>
    {
    }
}