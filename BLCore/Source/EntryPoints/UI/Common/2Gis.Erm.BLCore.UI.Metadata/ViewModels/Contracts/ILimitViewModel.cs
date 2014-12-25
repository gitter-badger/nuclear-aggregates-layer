using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface ILimitViewModel : IEntityViewModelAbstract<Limit>
    {
        LimitStatus Status { get; set; }
    }
}