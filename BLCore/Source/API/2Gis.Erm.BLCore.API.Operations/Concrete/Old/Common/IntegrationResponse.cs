namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common
{
    public sealed class IntegrationResponse : StreamResponse
    {
        public int ProcessedWithoutErrors { get; set; }
        public int BlockingErrorsAmount { get; set; }
        public int NonBlockingErrorsAmount { get; set; }

        public bool DoNotDisplayProcessingAmount { get; set; }
    }
}