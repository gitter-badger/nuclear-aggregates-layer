namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Documents
{
    public sealed class StringConditionCondition : ICondition
    {
        private readonly string _condition;

        public StringConditionCondition(string condition)
        {
            _condition = condition;
        }

        public bool EvaluateCondition(object document)
        {
            return true;
        }
    }
}