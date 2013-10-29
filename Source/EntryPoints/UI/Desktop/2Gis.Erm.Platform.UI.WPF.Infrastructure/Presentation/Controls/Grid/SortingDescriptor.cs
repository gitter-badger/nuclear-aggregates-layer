using System.ComponentModel;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid
{
    public sealed class SortingDescriptor
    {
        public string Column { get; set; }
        public ListSortDirection? Direction { get; set; }
    }
}