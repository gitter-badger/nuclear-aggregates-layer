using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts
{
    public interface ILegalPersonProfileViewModel : IEntityViewModelBase
    {
        bool IsMainProfile { get; set; }
    }
}