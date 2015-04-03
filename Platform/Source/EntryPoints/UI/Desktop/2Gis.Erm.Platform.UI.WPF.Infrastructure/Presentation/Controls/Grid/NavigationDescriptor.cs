using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid
{
    public sealed class NavigationDescriptor
    {
        public IEntityType EntityName { get; set; }
        public long EntityId { get; set; }
    }
}