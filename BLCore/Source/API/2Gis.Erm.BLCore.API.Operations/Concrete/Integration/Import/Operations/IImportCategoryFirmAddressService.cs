using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import.Operations
{
    public interface IImportCategoryFirmAddressService : IOperation<ImportCategoryFirmAddressIdentity>
    {
        void Import(IEnumerable<CategoryFirmAddress> categoryFirmAddressesToImport, IEnumerable<long> firmAddressCodes);
    }
}