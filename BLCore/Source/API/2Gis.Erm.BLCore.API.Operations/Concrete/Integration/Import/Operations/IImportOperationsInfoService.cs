using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.FinancialData1C;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AccountDetail;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import.Operations
{
    public interface IImportOperationsInfoService : IImportServiceBusDtoService<OperationsInfoServiceBusDto>, IOperation<ImportOperationsInfoIdentity>
    {
    }
}