using System;
using System.Linq;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts
{
    public static class OrderViewModelExtensions
    {
        public static void LockToolbar(this ICustomizableOrderViewModel orderViewModel)
        {
            Array.ForEach(orderViewModel.ViewConfig.CardSettings.CardToolbar.ToArray(), item => item.Disabled = true);
            orderViewModel.IsWorkflowLocked = true;
        }
    }
}