using System;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Aspects.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts
{
    public static class OrderViewModelExtensions
    {
        public static void LockOrderToolbar(this EntityViewModelBase<Order> orderViewModel)
        {
            var orderWorkflowLockableAspect = (IOrderWorkflowLockableAspect)orderViewModel;
           
            Array.ForEach(orderViewModel.ViewConfig.CardSettings.CardToolbar.ToArray(), item => item.Disabled = true);
            orderWorkflowLockableAspect.IsWorkflowLocked = true;
        }
    }
}