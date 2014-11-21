using System;

using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Navigation.ViewModels;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Blendability.Navigation
{
    public static class FakeNavigationAreasProvider
    {
        public static Tuple<IContextualNavigationArea, INavigationArea[]> Areas
        {
            get
            {
                return new Tuple<IContextualNavigationArea, INavigationArea[]>(new ContextualNavigationArea(), OrdinaryAreas);
            }
        }

        public static INavigationArea[] OrdinaryAreas
        {
            get
            {
                return new INavigationArea[]
                {
                    //new OrdinaryNavigationArea("Продажи")
                    //{
                    //    Items = new INavigationItem[]
                    //                {
                    //                    new ContextualNavigationItem("Сделки"), 
                    //                    new ContextualNavigationItem("Клиенты"),
                    //                    new ContextualNavigationItem("Контакты"),
                    //                    new ContextualNavigationItem("Заказы"),
                    //                    new ContextualNavigationItem("Фирмы"),
                    //                }
                    //},
                    //new OrdinaryNavigationArea("Отчеты")
                    //{
                    //    Items = new INavigationItem[]
                    //                {
                    //                    new ContextualNavigationItem("Ежедневное планирование менеджера"), 
                    //                    new ContextualNavigationItem("Выгрузки по лицевым счетам"),
                    //                    new ContextualNavigationItem("Клиенты без сделок")
                    //                }
                    //},
                    //new OrdinaryNavigationArea("Параметры")
                    //{
                    //    Items = new INavigationItem[]
                    //                {
                    //                    new ContextualNavigationItem("Администрирование"), 
                    //                    new ContextualNavigationItem("Управление катологом"),
                    //                    new ContextualNavigationItem("Управление очередью сообщений")
                    //                }
                    //}
                };
            }
        }
    }
}
