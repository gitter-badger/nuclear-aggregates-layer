using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.AutoMailer;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LocalMessages
{
    public sealed class ExportLocalMessageRequest : Request
    {
        public IntegrationTypeExport IntegrationType { get; set; }

        public MailSendingType SendingType { get; set; }
        public long? OrganizationUnitId { get; set; }
        public DateTime PeriodStart { get; set; }
        public bool IncludeRegionalAdvertisement { get; set; }
    }
}