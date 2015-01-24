using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IContactViewModel : IViewModelAbstract
    {
        // COMMENT {all, 24.01.2015}: DisplayName?
        string FullName { get; set; }
    }
}