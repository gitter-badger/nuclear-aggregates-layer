using System;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Models
{
    public sealed class SetLimitStatusViewModel : ViewModel
    {
        public long Id { get; set; }
        public Guid[] CrmIds { get; set; }
        public LimitStatus Status { get; set; }
    }
}