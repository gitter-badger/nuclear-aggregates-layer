using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public class AllClientsDebtReportViewModel : ViewModel
    {
        [RequiredLocalized]
        public LookupField Owner { get; set; }
        
        [RequiredLocalized]
        public LookupField OrganizationUnit { get; set; }
        
        public bool WithPaymentDebt { get; set; }
        
        public bool WithDocDebtOrder { get; set; }
        
        public bool WithDocDebtBargain { get; set; }
        
        public bool WithDocDebtTermination { get; set; }
    }
}