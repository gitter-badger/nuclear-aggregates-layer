using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IPricePositionViewModel : IEntityViewModelAbstract<PricePosition>
    {
        LookupField Position { get; set; }
    }
}