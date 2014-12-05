using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts
{
    public interface ICustomizableLegalPersonProfileViewModel : IEntityViewModelBase
    {
        bool IsMainProfile { get; set; }
    }
}