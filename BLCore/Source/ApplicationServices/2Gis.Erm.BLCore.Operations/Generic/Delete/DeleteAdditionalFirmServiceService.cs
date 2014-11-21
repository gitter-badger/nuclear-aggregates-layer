using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Firms;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Delete
{
    public sealed class DeleteAdditionalFirmServiceService : IDeleteGenericEntityService<AdditionalFirmService>
    {
        private readonly IAdditionalFirmServicesService _firmService;

        public DeleteAdditionalFirmServiceService(IAdditionalFirmServicesService firmService)
        {
            _firmService = firmService;
        }

        public DeleteConfirmation Delete(long entityId)
        {
            _firmService.Delete(entityId);
            return null;
        }

        public DeleteConfirmationInfo GetConfirmation(long entityId)
        {
            return null;
        }
    }
}