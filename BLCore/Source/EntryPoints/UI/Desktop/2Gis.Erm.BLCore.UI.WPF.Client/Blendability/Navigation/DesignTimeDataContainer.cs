using System.Linq;

using DoubleGis.Erm.BLCore.UI.WPF.Client.Blendability.Navigation;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Navigation.ViewModels;
using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources.Titles;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Localization;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Blendability
// ReSharper restore CheckNamespace
{
    public static partial class DesignTimeDataContainer
    {
        public static class Navigation
        {
            public static INavigationArea NavigationArea
            {
                get
                {
                    return FakeNavigationAreasProvider.OrdinaryAreas.First();
                }
            }

            public static INavigationArea ContextualNavigationArea
            {
                get
                {
                    var area = new ContextualNavigationArea();
                    var items = new INavigationItem[]
                                    {
                                        new FakeNavigationItem("Сведения")
                                            {
                                                Items =
                                                    new INavigationItem[]
                                                        {
                                                            new FakeNavigationItem("Основные"), 
                                                            new FakeNavigationItem("Дополнительно"),
                                                            new FakeNavigationItem("Администрирование"), 
                                                            new FakeNavigationItem("Примечание"),
                                                            new FakeNavigationItem("История изменений")
                                                        }
                                            },
                                        new FakeNavigationItem("Счета на оплату")
                                            {
                                                //IconKey = Images.Navigation.NavAreaBilling
                                            }, 
                                        new FakeNavigationItem("Блокировки"), 
                                        new FakeNavigationItem("Файлы к заказу")
                                    };
                    area.UpdateContext(new StaticTitleProvider(new StaticTitleDescriptor("Card")), items);
                    return area;
                }
            }
        }
    }
}
