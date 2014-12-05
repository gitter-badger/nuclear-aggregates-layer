using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts
{
    public interface ICustomizableFirmViewModel : IEntityViewModelBase
    {
        bool ClosedForAscertainment { get; set; }
    }
}