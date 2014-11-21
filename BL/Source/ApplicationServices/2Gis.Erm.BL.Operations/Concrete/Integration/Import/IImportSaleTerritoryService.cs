using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Georgaphy;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Territory;

namespace DoubleGis.Erm.BL.Operations.Concrete.Integration.Import
{
    public interface IImportSaleTerritoryService : IImportServiceBusDtoService<SaleTerritoryServiceBusDto>, IOperation<ImportSaleTerritoryIdentity>
    {
    }
}