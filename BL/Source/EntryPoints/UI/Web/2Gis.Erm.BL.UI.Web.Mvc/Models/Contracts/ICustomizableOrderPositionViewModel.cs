using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts
{
    public interface ICustomizableOrderPositionViewModel : IEntityViewModelBase
    {
        long OrderId { get; set; }
        bool IsBlockedByRelease { get; set; }
        bool IsRated { get; set; }
        decimal CategoryRate { get; set; }
        decimal DiscountPercent { get; set; }
        int MoneySignificantDigitsNumber { get; set; }
    }
}