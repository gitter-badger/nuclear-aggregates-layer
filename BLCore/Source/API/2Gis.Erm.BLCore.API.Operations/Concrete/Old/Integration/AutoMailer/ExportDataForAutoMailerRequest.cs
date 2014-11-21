using System;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.AutoMailer
{
    public class ExportDataForAutoMailerRequest : Request
    {
        public DateTime PeriodStart { get; set; }
        public MailSendingType SendingType { get; set; }
        public bool IncludeRegionalAdvertisement { get; set; }
    }
}
 