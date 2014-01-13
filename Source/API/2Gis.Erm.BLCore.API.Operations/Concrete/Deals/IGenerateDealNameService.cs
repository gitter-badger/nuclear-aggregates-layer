using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Deal;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Deals
{
    // 2+ \BL\Source\API\2Gis.Erm.BLCore.API.Operations\Concrete\Deals\IGenerateDealNameService.cs
    public interface IGenerateDealNameService : IOperation<GenerateDealNameIdentity>
    {
        string GenerateDealName(string clientName, string mainFirmName);

        string GenerateDealName(long clientId);
    }
}
