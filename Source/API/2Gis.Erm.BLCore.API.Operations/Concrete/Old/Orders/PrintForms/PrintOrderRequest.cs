using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms
{
	public sealed class PrintOrderRequest : Request
	{
		public long OrderId { get; set; }
		public bool PrintRegionalVersion { get; set; }
        public long? LegalPersonProfileId { get; set; }
	}
}