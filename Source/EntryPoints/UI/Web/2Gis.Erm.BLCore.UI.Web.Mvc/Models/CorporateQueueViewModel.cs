using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Models
{
    public sealed class CorporateQueueViewModel : ViewModel
    {
        public string FromAdsToBillingQueueName { get; set; }
        public int FromAdsToBillingQueueCount { get; set; }

        public string FromDgppToBillingQueueName { get; set; }
        public int FromDgppToBillingQueueCount { get; set; }

        public string FromBillingToAdsQueueName { get; set; }
        public int FromBillingToAdsQueueCount { get; set; }
    }
}