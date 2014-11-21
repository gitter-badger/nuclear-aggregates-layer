using DoubleGis.Erm.Platform.Common.Crosscutting;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting
{
    public interface IEvaluateBargainNumberService : IInvariantSafeCrosscuttingService
    {
        string Evaluate(BargainKind bargainKind, string legalPersonOrganizationUnitCode, string branchOfficeOrganizationUnitCode, long bargainUniqueIndex);
    }
}