using DoubleGis.Erm.BLCore.UI.Metadata.Aspects;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface ILimitViewModel : INewableAspect
    {
        LimitStatus Status { get; set; }
    }
}