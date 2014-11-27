using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts
{
    public interface IBargainViewModel : IEntityViewModelBase<Bargain>
    {
        string Number { get; set; }
    }
}
