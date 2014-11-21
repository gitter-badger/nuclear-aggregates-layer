namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Conditions
{
    public sealed class StringConditionCondition : ICondition
    {
        private readonly string _condition;

        public StringConditionCondition(string condition)
        {
            _condition = condition;
        }

        public string Condition
        {
            get { return _condition; }
        }

        public bool EvaluateCondition(object document)
        {
            return true;
        }
    }
}