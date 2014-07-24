using System;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Bargain;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders.Bargains
{
    public interface IDetermineOrderBargainOperationService : IOperation<DetermineOrderBargainIdentity>
    {
        bool TryDetermineOrderBargain(long legalPersonId,
                                      long branchOfficeOrganizationUnitId,
                                      DateTime orderEndDistributionDate,
                                      out long bargainId,
                                      out string bargainNumber);
    }
}