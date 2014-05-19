using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import
{
    public interface IImportFlowCardsForErmOperationService : IOperation<ImportCardForErmIdentity>
    {
        void Import();
    }
}