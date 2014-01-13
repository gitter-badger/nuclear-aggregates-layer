using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old
{
    public sealed class EditOrderPositionRequest : EditRequest<OrderPosition>
    {
        public AdvertisementDescriptor[] AdvertisementsLinks { get; set; }
        public bool HasUpdatedAdvertisementMaterials { get; set; }

        public EditOrderPositionRequest()
        {
            HasUpdatedAdvertisementMaterials = true;
        }
    }
}