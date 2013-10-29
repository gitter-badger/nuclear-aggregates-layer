using System;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.ViewModels;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.ViewResolvers;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Components
{
    /// <summary>
    /// Интерфейс компонента, предназначенного для размещения в какой-то целевой (конкретной/специфической) области Layout Shell
    /// Компонент - это агрегирующая сущность, объединяющая значние о типе viewmodel, со способом получить UI для отображения этой viewmodel,
    /// а также с целевым регионом для отображения нужного UI.
    /// Определять целевую область Layout Shell для конкретного типа компонента, пока предполагается, с помощью интерфейсов индикаторов,
    /// т.е. для каждой целевой области, поддерживаемой Layout Shell, нужно объявлять интерфейс расширяющий generic версию данного (
    /// возможно он будет не маркерный а иметь какой-то специфический контракт для конкретной области).
    /// В конкретной реализации под UI для отображения понимается - получить template для view представляющего эту viewmodel.
    /// Т.о. видно, что указывая для одного и того же типа viewmodel, разные viewresolver и целевые регионы, можно обеспечить отображение
    /// одной и той же viewmodel в различном UI виде, в разных частях shell
    /// </summary>
    public interface ILayoutComponent
    {
        Guid ComponentId { get; }
        Type ViewModelType { get; }
        ILayoutComponentViewResolver ViewResolver { get; }
    }

    /// <summary>
    /// Накладывает ограничение на тип viewmodel описываемых компонентом - они все обязаны расширять какой-то общий для них интерфейс
    /// </summary>
    public interface ILayoutComponent<TViewModelIndicator> : ILayoutComponent
        where TViewModelIndicator : ILayoutComponentViewModel
    {
    }
}
