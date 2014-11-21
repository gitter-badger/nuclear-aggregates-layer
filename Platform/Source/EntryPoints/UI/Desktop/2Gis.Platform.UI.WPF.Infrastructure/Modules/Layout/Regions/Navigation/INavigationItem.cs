using System;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;
using DoubleGis.Platform.UI.WPF.Infrastructure.MVVM;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation
{
    /// <summary>
    /// Элемент списка навигации, позволяет viewmodel публиковать команды, вызываемые при выборе соответствующего пункта в навигационной панели 
    /// </summary>
    public interface INavigationItem
    {
        Uri Id { get; }
        string Title { get; }
        IImageProvider Icon { get; }
        IDelegateCommand NavigateCommand { get; }

        INavigationItem[] Items { get; }

        bool IsSelected { get; set; }
        bool IsExpanded { get; set; }
    }
}
