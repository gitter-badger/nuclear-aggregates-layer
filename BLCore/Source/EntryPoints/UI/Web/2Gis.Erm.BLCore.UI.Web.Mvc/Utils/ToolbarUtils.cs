using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings.ConfigurationDto;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Utils
{
    public static class ToolbarUtils
    {
        public static void DisableButtons(this IEnumerable<DataViewJson> dataViews, params string[] buttons)
        {
            var buttonsToDisable = dataViews.SelectMany(x => x.ToolbarItems).Where(item => buttons.Contains(item.Name, StringComparer.OrdinalIgnoreCase));
            foreach (var item in buttonsToDisable)
            {
                item.Disabled = true;
            }
        }
    }
}