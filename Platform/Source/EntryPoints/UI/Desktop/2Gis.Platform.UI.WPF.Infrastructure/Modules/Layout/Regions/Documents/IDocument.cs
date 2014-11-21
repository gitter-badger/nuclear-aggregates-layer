using System;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.ViewModels;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents
{
    /// <summary>
    /// Документ - базовый интерфейс для любой viewmodel, которая хочет отображаться в content area контрола с вкладками (карточки, navigation grid и т.п.)
    /// </summary>
    public interface IDocument : ILayoutComponentViewModel
    {
        string DocumentName { get; }
        Guid Id { get; }
    }
}
