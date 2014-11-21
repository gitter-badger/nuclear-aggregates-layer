using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts
{
    public interface ICustomizableLegalPersonViewModel : IEntityViewModelBase
    {
        bool HasProfiles { get; set; }
    }
}