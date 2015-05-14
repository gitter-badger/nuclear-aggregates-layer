using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card
{
    public interface ICardViewModelIdentity : IViewModelIdentity
    {
        long EntityId { get; set; }
        IEntityType EntityName { get; }
        IOperationIdentity OperationIdentity { get; }
    }
}
