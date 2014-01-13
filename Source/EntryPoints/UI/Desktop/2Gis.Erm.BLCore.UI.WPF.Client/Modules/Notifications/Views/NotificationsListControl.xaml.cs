using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Notifications.Views
{
    /// <summary>
    /// Interaction logic for NotificationsList.xaml
    /// </summary>
    public partial class NotificationsListControl : UserControl
    {
        public NotificationsListControl()
        {
            InitializeComponent();
        }

        private void ListView_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            const int PaddingRight = 10;
            if (!e.WidthChanged)
            {
                return;
            }

            var listView = (ListView)sender;
            var gridView = (GridView)listView.View;
            
            var border = VisualTreeHelper.GetChild(listView, 0) as Decorator;
            if (border == null)
            {
                return;
            }

            var scroller = border.Child as ScrollViewer;
            if (scroller == null)
            {
                return;
            }

            var presenter = scroller.Content as ItemsPresenter;
            if (presenter == null)
            {
                return;
            }

            var columnCount = gridView.Columns.Count;
            if (columnCount > 0)
            {
                gridView.Columns[columnCount - 1].Width = presenter.ActualWidth;
                for (int i = 0; i < columnCount - 1; i++)
                {
                    gridView.Columns[columnCount - 1].Width -= gridView.Columns[i].ActualWidth;
                }

                // Padding
                gridView.Columns[columnCount - 1].Width -= PaddingRight;
            }
        }
    }
}
