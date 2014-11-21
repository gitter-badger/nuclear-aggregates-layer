using System;
using System.Collections.Generic;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Navigation.ViewModels
{
    public static class NavigationItemUtils
    {
        public static bool Do(
            this IEnumerable<INavigationItem> items,
            Func<INavigationItem, bool> checker,
            bool isGreedy,
            Action<INavigationItem> applyMethod)
        {
            foreach (var item in items)
            {
                if (item == null)
                {
                    continue;
                }

                if (checker(item))
                {
                    applyMethod(item);
                    if (!isGreedy)
                    {
                        return true;
                    }
                }

                if (item.Items == null)
                {
                    continue;
                }

                if (item.Items.Do(checker, isGreedy, applyMethod))
                {
                    if (!isGreedy)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static void ProcessAll(this IEnumerable<INavigationItem> items, Action<INavigationItem> applyMethod)
        {
            foreach (var item in items)
            {
                if (item == null)
                {
                    continue;
                }

                applyMethod(item);

                if (item.Items == null)
                {
                    continue;
                }

                item.Items.ProcessAll(applyMethod);
            }
        }
    }
}