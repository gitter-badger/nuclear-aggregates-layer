using System.Linq;

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

        // TODO {all, 20.02.2014}: После обновления UI позиции заказа имеет смысл реализацию свойства поменять на auto
        public long? CategoryId
        {
            get
            {
                var categories = AdvertisementsLinks.Where(x => x.CategoryId != null).Select(x => x.CategoryId.Value).Distinct().ToArray();
                return categories.Length == 1 ? (long?)categories[0] : null;
            }
        }
    }
}