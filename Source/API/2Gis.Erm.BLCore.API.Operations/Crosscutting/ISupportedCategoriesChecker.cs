using DoubleGis.Erm.Platform.Common.Crosscutting;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Operations.Crosscutting
{
    public interface ISupportedCategoriesChecker : IInvariantSafeCrosscuttingService
    {
        bool IsSupported(PricePositionRateType rateType, long categoryId, long destOrganizationUnitId);
        void Check(PricePositionRateType rateType, long categoryId, long destOrganizationUnitId);
    }
}