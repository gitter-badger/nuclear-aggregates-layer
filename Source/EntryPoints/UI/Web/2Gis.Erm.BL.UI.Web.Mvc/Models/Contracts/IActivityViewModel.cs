using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts
{
    public interface IActivityViewModel : IEntityViewModelBase
    {
        ActivityStatus Status { get; set; }
    }
}