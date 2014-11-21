using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card
{
    public interface ICardViewModelIdentity : IViewModelIdentity
    {
        long EntityId { get; set; }
        EntityName EntityName { get; }
        IOperationIdentity OperationIdentity { get; }
    }
}
