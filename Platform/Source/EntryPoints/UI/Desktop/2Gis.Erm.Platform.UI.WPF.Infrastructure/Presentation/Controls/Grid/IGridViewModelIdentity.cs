using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid
{
    public interface IGridViewModelIdentity : IViewModelIdentity
    {
        EntityName EntityName { get; }
    }
}