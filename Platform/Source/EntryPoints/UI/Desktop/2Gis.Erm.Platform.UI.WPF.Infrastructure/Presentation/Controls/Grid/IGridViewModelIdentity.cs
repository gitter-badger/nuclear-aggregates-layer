using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid
{
    public interface IGridViewModelIdentity : IViewModelIdentity
    {
        IEntityType EntityName { get; }
    }
}