using System;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills
{
    public sealed class GetRelatedOrdersForCreateBillRequest : Request
    {
        public long OrderId { get; set; }
    }

    public sealed class GetRelatedOrdersForCreateBillResponse : Response
    {
        public RelatedOrderDescriptor[] Orders { get; set; }
    }

    public sealed class RelatedOrderDescriptor
    {
        public long Id { get; set; }
        public string Number { get; set; }
        public DateTime BeginDistributionDate { get; set; }
        public DateTime EndDistributionDate { get; set; }
        public String SourceOrganizationUnit { get; set; }
        public String DestinationOrganizationUnit { get; set; }
    }
}
