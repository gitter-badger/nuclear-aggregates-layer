using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Firms
{
    public interface IAdditionalFirmServicesService : ISimplifiedModelConsumer
    {
        void CreateOrUpdate(AdditionalFirmService firmService);
        void Delete(AdditionalFirmService firmService);
        void Delete(long entityId);
    }
}
