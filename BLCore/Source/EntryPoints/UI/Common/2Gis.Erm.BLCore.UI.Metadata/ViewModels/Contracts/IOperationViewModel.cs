using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IOperationViewModel : IEntityViewModelAbstract<Operation>
    {
        BusinessOperation Type { get; set; }
    }
}