using System;
using System.Collections.Generic;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Notifications;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents
{
    public class DocumentValidatedEventArgs : EventArgs
    {
        public IEnumerable<INotification> Notifications { get; set; }
    }
}