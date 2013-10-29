using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;
using DoubleGis.Platform.UI.WPF.Infrastructure.Util;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Toolbar
{
    public sealed class ToolbarItemViewSelector : DataTemplateSelector
    {
        private readonly Type _targetViewModelType;
        private readonly Lazy<DataTemplate> _singularItemTemplate;
        private readonly Lazy<DataTemplate> _hierarchyItemTemplate;

        public ToolbarItemViewSelector()
        {
            _targetViewModelType = typeof(INavigationItem);

            #region !!! Зачем здесь Lazy
            // Конструктор данного класса может вызываться из произвольного потока (совсем не обязательно из UI)
            // поэтому создавать экземпляр шаблона DataTemplate в этом методе нельзя, т.к. он будет инфраструктурой WPF связан с потоком,
            // после чего если в SelectTemplate (который обычно вызывается из UI потока) попытаться его вернуть - будут проблемы с многопоточностью
            // Exception будет гласить - The calling thread cannot access this object because a different thread owns it 
            // Решение - сделать так, чтобы инстанцирование происходило в контексте UI потока, т.е. в момент вызова SelectTemplate
            // поэтому, либо пересоздавать шаблон при каждом вызове, либо кэшировать, но правильно
            #endregion
            _singularItemTemplate = new Lazy<DataTemplate>(() => TemplateUtils.CreateDataTemplate(_targetViewModelType, typeof(SingularToolbarItemControl)));
            _hierarchyItemTemplate = new Lazy<DataTemplate>(() => TemplateUtils.CreateDataTemplate(_targetViewModelType, typeof(HierarchyToolbarItemControl)));
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null)
            {
                return base.SelectTemplate(null, container);
            }

            var toobarItem = item as INavigationItem;
            if (toobarItem == null)
            {
                throw new InvalidOperationException("Not supported toolbar item type " + item.GetType() + ". Toolbar item have to implement interface " + _targetViewModelType.Name);
            }

            if (toobarItem.Items == null || !toobarItem.Items.Any())
            {
                return _singularItemTemplate.Value;
            }

            return _hierarchyItemTemplate.Value;
        }
    }
}
