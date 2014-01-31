namespace DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Crosscutting
{
    public sealed class EvaluateBillNumberService : IEvaluateBillNumberService
    {
        private readonly string _billNumberTemplate;

        public EvaluateBillNumberService(string billNumberTemplate)
        {
            _billNumberTemplate = billNumberTemplate;
        }
        
        public string Evaluate(string orderNumber)
        {
            return string.Format(_billNumberTemplate, orderNumber);
        }

        public string Evaluate(string orderNumber, int billIndex)
        {
            return string.Format(_billNumberTemplate, orderNumber + "/" + billIndex);
        }
    }
}