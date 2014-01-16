namespace DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Crosscutting
{
    public sealed class EvaluateBargainNumberService : IEvaluateBargainNumberService
    {
        private readonly string _bargainNumberTemplate;

        public EvaluateBargainNumberService(string bargainNumberTemplate)
        {
            _bargainNumberTemplate = bargainNumberTemplate;
        }

        public string Evaluate(string sourceOrganizationUnitSyncCode1C, string destinationOrganizationUnitSyncCode1C, long bargainUniqueIndex)
        {
            return string.Format(_bargainNumberTemplate, sourceOrganizationUnitSyncCode1C, destinationOrganizationUnitSyncCode1C, bargainUniqueIndex);
        }
    }
}