
namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration
{
    public sealed class ImportResponse : MessageProcessingResponse
    {
        public int Total { get; set; }
        public int Processed { get; set; }

        // otrganization unit determined after processing, refactor this
        public long? OrganizationUnitId { get; set; }
    }
}