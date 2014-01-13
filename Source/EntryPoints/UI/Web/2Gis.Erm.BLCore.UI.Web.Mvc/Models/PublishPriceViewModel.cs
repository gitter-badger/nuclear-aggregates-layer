using System;

using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Models
{
    public sealed class PublishPriceViewModel : ViewModel
    {
        public long Id { get; set; }
        public long OrganizationUnitId { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime PublishDate { get; set; }
    }
}
